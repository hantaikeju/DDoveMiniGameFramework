using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DDoveFramework.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DDoveFramework.Extension.DDoveUI
{
    public static partial class DDoveUIKit
    {
        private static DDoveUIInitInfo _config;
        private static GameObject _uiRoot;
        private static GameObject _cacheRoot;
        private static Camera _uiCamera;
        private static Canvas _canvas;
        private static bool _initialized;

        private static readonly Dictionary<DDoveUILayer, RectTransform> Layers = new Dictionary<DDoveUILayer, RectTransform>();
        private static readonly Dictionary<string, IDDoveUIPanel> ActivePanels = new Dictionary<string, IDDoveUIPanel>();
        private static readonly Stack<string> PanelStack = new Stack<string>();
        private static readonly HashSet<string> Opening = new HashSet<string>();

        public static GameObject UIRoot => _uiRoot;

        public static DDoveUIInitInfo Config
        {
            get
            {
                if (_config == null)
                {
                    _config = DDoveUIInitInfo.Load();
                    if (_config == null)
                    {
                        DDoveDebug.LogWarning(DDoveUIInitInfo.LogTitle, ("reason", "missing DDoveUIInitInfo, using defaults"));
                        _config = ScriptableObject.CreateInstance<DDoveUIInitInfo>();
                    }
                }

                return _config;
            }
        }

        public static void Initialize()
        {
            if (_initialized)
            {
                DDoveDebug.LogWarning(DDoveUIInitInfo.LogTitle, ("reason", "already initialized"));
                return;
            }

            _uiRoot = GameObject.Find("DDoveUIRoot");
            if (_uiRoot == null)
            {
                _uiRoot = new GameObject("DDoveUIRoot", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                var uiLayer = LayerMask.NameToLayer("UI");
                _uiRoot.layer = uiLayer >= 0 ? uiLayer : 5;
            }

            UnityEngine.Object.DontDestroyOnLoad(_uiRoot);

            _canvas = GetOrAdd<Canvas>(_uiRoot);
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;

            var scaler = GetOrAdd<CanvasScaler>(_uiRoot);
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = Config.ReferenceResolution;
            scaler.matchWidthOrHeight = Config.MatchWidthOrHeight;
            scaler.referencePixelsPerUnit = Config.ReferencePixelsPerUnit;

            GetOrAdd<GraphicRaycaster>(_uiRoot);
            InitCamera();
            InitCacheRoot();
            InitLayers();
            EnsureEventSystem();
            _initialized = true;
        }

        public static RectTransform GetLayer(DDoveUILayer layer)
        {
            EnsureInitialized();
            return Layers.TryGetValue(layer, out var rect) ? rect : null;
        }

        public static async UniTask<T> OpenAsync<T>(IDDoveUIPanelData data = null) where T : DDoveUIPanelBase<T>
        {
            EnsureInitialized();
            var panelName = typeof(T).Name;
            if (ActivePanels.TryGetValue(panelName, out var existing))
            {
                existing.Show();
                return existing as T;
            }

            if (TryPopFromCache<T>(panelName, out var cached))
            {
                cached.transform.SetParent(GetLayer(cached.DefaultLayer), false);
                ActivePanels[panelName] = cached;
                cached.Show();
                return cached;
            }

            if (!Opening.Add(panelName))
            {
                DDoveDebug.LogWarning(DDoveUIInitInfo.LogTitle, ("panel", panelName), ("reason", "already opening"));
                return null;
            }

            try
            {
                var prefab = await LoadPanelPrefabAsync(panelName);
                if (prefab == null)
                {
                    return null;
                }

                var instance = UnityEngine.Object.Instantiate(prefab);
                var panel = instance.GetComponent<T>() ?? instance.AddComponent<T>();
                if (panel == null)
                {
                    DDoveDebug.LogError(DDoveUIInitInfo.LogTitle, ("panel", panelName), ("reason", "cannot add panel component"));
                    UnityEngine.Object.Destroy(instance);
                    return null;
                }

                instance.transform.SetParent(GetLayer(panel.DefaultLayer), false);
                instance.SetActive(true);
                ActivePanels[panelName] = panel;
                await panel.OpenAsync(data);
                return panel;
            }
            catch (Exception e)
            {
                DDoveDebug.LogError(DDoveUIInitInfo.LogTitle, ("panel", panelName), ("error", e.Message));
                return null;
            }
            finally
            {
                Opening.Remove(panelName);
            }
        }

        public static void Close<T>() where T : DDoveUIPanelBase<T>
        {
            Close(typeof(T));
        }

        public static void Close(Type type)
        {
            var panelName = type.Name;
            RemoveFromCache(panelName);
            if (!ActivePanels.TryGetValue(panelName, out var panel))
            {
                return;
            }

            ActivePanels.Remove(panelName);
            RemoveFromPanelStack(panelName);
            panel.Close();
            OnPanelClosed(panelName);
        }

        public static T GetPanel<T>() where T : DDoveUIPanelBase<T>
        {
            return ActivePanels.TryGetValue(typeof(T).Name, out var panel) ? panel as T : null;
        }

        public static async UniTask NavigateToAsync<T>(IDDoveUIPanelData data = null) where T : DDoveUIPanelBase<T>
        {
            EnsureInitialized();
            var panelName = typeof(T).Name;
            if (PanelStack.Count > 0 && PanelStack.Peek() == panelName)
            {
                return;
            }

            if (PanelStack.Contains(panelName))
            {
                await BackToAsync<T>();
                return;
            }

            if (PanelStack.Count > 0 && ActivePanels.TryGetValue(PanelStack.Peek(), out var top))
            {
                top.Hide();
            }

            await OpenAsync<T>(data);
            PanelStack.Push(panelName);
        }

        public static async UniTask BackAsync(bool showNext = true)
        {
            if (PanelStack.Count == 0)
            {
                DDoveDebug.LogWarning(DDoveUIInitInfo.LogTitle, ("reason", "empty history"));
                return;
            }

            var topName = PanelStack.Pop();
            if (ActivePanels.TryGetValue(topName, out var top))
            {
                top.Hide();
                if (top.EnableClose)
                {
                    ActivePanels.Remove(topName);
                    if (!TryCachePanel(topName, top))
                    {
                        top.Close();
                        OnPanelClosed(topName);
                    }
                }
            }

            await UniTask.Yield();
            if (showNext && PanelStack.Count > 0 && ActivePanels.TryGetValue(PanelStack.Peek(), out var next))
            {
                next.Show();
            }
        }

        public static async UniTask BackToAsync<T>() where T : DDoveUIPanelBase<T>
        {
            var targetName = typeof(T).Name;
            while (PanelStack.Count > 0 && PanelStack.Peek() != targetName)
            {
                await BackAsync(false);
            }

            if (PanelStack.Count > 0 && ActivePanels.TryGetValue(targetName, out var target))
            {
                target.Show();
            }
        }

        public static async UniTask ClearHistoryAsync()
        {
            while (PanelStack.Count > 0)
            {
                await BackAsync(false);
            }
        }

        public static async UniTask OpenExclusiveAsync<T>(IDDoveUIPanelData data = null) where T : DDoveUIPanelBase<T>
        {
            var panelName = typeof(T).Name;
            if (!ActivePanels.ContainsKey(panelName))
            {
                await OpenAsync<T>(data);
            }
            else if (ActivePanels.TryGetValue(panelName, out var panel))
            {
                panel.Show();
            }

            CloseAllExcept<T>();
        }

        public static void CloseAllExcept<T>() where T : DDoveUIPanelBase<T>
        {
            var keep = typeof(T).Name;
            ClearLRUCache();
            foreach (var name in ActivePanels.Keys.ToArray())
            {
                if (name == keep)
                {
                    continue;
                }

                if (!ActivePanels.TryGetValue(name, out var panel))
                {
                    continue;
                }

                panel.Hide();
                if (!panel.EnableClose)
                {
                    continue;
                }

                panel.Close();
                ActivePanels.Remove(name);
                OnPanelClosed(name);
            }

            PanelStack.Clear();
        }

        public static void CloseAll()
        {
            ClearLRUCache();
            foreach (var name in ActivePanels.Keys.ToArray())
            {
                if (!ActivePanels.TryGetValue(name, out var panel))
                {
                    continue;
                }

                panel.Hide();
                if (!panel.EnableClose)
                {
                    continue;
                }

                panel.Close();
                ActivePanels.Remove(name);
                OnPanelClosed(name);
            }

            PanelStack.Clear();
        }

        public static bool IsPanelOpen<T>() where T : DDoveUIPanelBase<T>
        {
            return ActivePanels.ContainsKey(typeof(T).Name);
        }

        public static bool IsPanelInStack<T>() where T : DDoveUIPanelBase<T>
        {
            return PanelStack.Contains(typeof(T).Name);
        }

        public static string GetCurrentPanelName()
        {
            return PanelStack.Count > 0 ? PanelStack.Peek() : null;
        }

        public static int GetHistoryCount()
        {
            return PanelStack.Count;
        }

        private static void EnsureInitialized()
        {
            if (!_initialized)
            {
                Initialize();
            }
        }

        private static void InitCamera()
        {
            var camTransform = _uiRoot.transform.Find("UICamera");
            if (camTransform == null)
            {
                var cameraGo = new GameObject("UICamera", typeof(Camera));
                cameraGo.transform.SetParent(_uiRoot.transform, false);
                _uiCamera = cameraGo.GetComponent<Camera>();
            }
            else
            {
                _uiCamera = camTransform.GetComponent<Camera>() ?? camTransform.gameObject.AddComponent<Camera>();
            }

            const float planeDistance = 100f;
            _uiCamera.clearFlags = CameraClearFlags.Depth;
            _uiCamera.depth = Config.UiCameraDepth;
            var uiLayer = LayerMask.NameToLayer("UI");
            _uiCamera.cullingMask = 1 << (uiLayer >= 0 ? uiLayer : 5);
            _uiCamera.orthographic = true;
            _uiCamera.transform.localPosition = new Vector3(0f, 0f, -planeDistance);
            _uiCamera.nearClipPlane = 0.3f;
            _uiCamera.farClipPlane = planeDistance * 2f;
            _canvas.worldCamera = _uiCamera;
            _canvas.planeDistance = planeDistance;
        }

        private static void InitCacheRoot()
        {
            var cache = _uiRoot.transform.Find("DDoveUICacheRoot");
            _cacheRoot = cache != null ? cache.gameObject : new GameObject("DDoveUICacheRoot");
            if (cache == null)
            {
                _cacheRoot.transform.SetParent(_uiRoot.transform, false);
            }

            _cacheRoot.SetActive(false);
        }

        private static void InitLayers()
        {
            Layers.Clear();
            foreach (DDoveUILayer layer in Enum.GetValues(typeof(DDoveUILayer)))
            {
                var name = layer.ToString();
                var existing = _uiRoot.transform.Find(name);
                if (existing == null)
                {
                    var go = new GameObject(name, typeof(RectTransform));
                    go.transform.SetParent(_uiRoot.transform, false);
                    var rect = go.GetComponent<RectTransform>();
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    rect.sizeDelta = Vector2.zero;
                    rect.anchoredPosition = Vector2.zero;
                    existing = rect;
                }

                Layers[layer] = existing as RectTransform;
            }
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null)
            {
                return;
            }

            var go = new GameObject("EventSystem");
            go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
            UnityEngine.Object.DontDestroyOnLoad(go);
        }

        private static void RemoveFromPanelStack(string panelName)
        {
            if (!PanelStack.Contains(panelName))
            {
                return;
            }

            var items = PanelStack.ToList();
            items.Remove(panelName);
            PanelStack.Clear();
            items.Reverse();
            foreach (var name in items)
            {
                PanelStack.Push(name);
            }
        }

        private static T GetOrAdd<T>(GameObject go) where T : Component
        {
            var component = go.GetComponent<T>();
            return component != null ? component : go.AddComponent<T>();
        }
    }
}
