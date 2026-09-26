using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DDoveFramework.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
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
        private static readonly Dictionary<string, List<string>> ChildPanels = new Dictionary<string, List<string>>();
        private static readonly Dictionary<string, string> ChildOwners = new Dictionary<string, string>();
        private static readonly HashSet<string> PanelsWithChildren = new HashSet<string>();

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
            ConfigureInputSystem();
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

        public static async UniTask<TChild> OpenChildAsync<TChild>(Component parent, IDDoveUIPanelData data = null)
            where TChild : DDoveUIPanelBase<TChild>
        {
            EnsureInitialized();
            var childName = typeof(TChild).Name;
            if (parent == null)
            {
                DDoveDebug.LogError(DDoveUIInitInfo.LogTitle, ("panel", childName), ("reason", "missing parent"));
                return null;
            }

            var parentName = parent.GetType().Name;
            if (IsChildOf(parentName, childName) && ActivePanels.TryGetValue(childName, out var opened))
            {
                HideSiblingChildren(parentName, childName);
                opened.Show();
                return opened as TChild;
            }

            if (ActivePanels.ContainsKey(childName))
            {
                DDoveDebug.LogError(DDoveUIInitInfo.LogTitle, ("panel", childName), ("reason", "already open"));
                return null;
            }

            var content = FindDirectContent(parent.transform);
            if (content == null)
            {
                DDoveDebug.LogError(DDoveUIInitInfo.LogTitle, ("panel", childName), ("parent", parentName), ("reason", "missing Content"));
                return null;
            }

            if (!Opening.Add(childName))
            {
                DDoveDebug.LogWarning(DDoveUIInitInfo.LogTitle, ("panel", childName), ("reason", "already opening"));
                return null;
            }

            GameObject instance = null;
            var tracked = false;
            try
            {
                if (TryPopFromCache<TChild>(childName, out var cached))
                {
                    cached.transform.SetParent(content, false);
                    ActivePanels[childName] = cached;
                    RegisterChild(parentName, childName);
                    HideSiblingChildren(parentName, childName);
                    cached.Show();
                    return cached;
                }

                var prefab = await LoadPanelPrefabAsync(childName);
                if (prefab == null)
                {
                    return null;
                }

                instance = UnityEngine.Object.Instantiate(prefab);
                var panel = instance.GetComponent<TChild>() ?? instance.AddComponent<TChild>();
                if (panel == null)
                {
                    DDoveDebug.LogError(DDoveUIInitInfo.LogTitle, ("panel", childName), ("reason", "cannot add panel component"));
                    UnityEngine.Object.Destroy(instance);
                    instance = null;
                    return null;
                }

                instance.transform.SetParent(content, false);
                instance.SetActive(true);
                ActivePanels[childName] = panel;
                tracked = true;
                RegisterChild(parentName, childName);
                HideSiblingChildren(parentName, childName);
                await panel.OpenAsync(data);
                return panel;
            }
            catch (Exception e)
            {
                if (!tracked && instance != null)
                {
                    UnityEngine.Object.Destroy(instance);
                }

                DDoveDebug.LogError(DDoveUIInitInfo.LogTitle, ("panel", childName), ("error", e.Message));
                return null;
            }
            finally
            {
                Opening.Remove(childName);
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

            CloseRegisteredChildren(panelName);
            ActivePanels.Remove(panelName);
            RemoveFromPanelStack(panelName);
            ReleasePanelInstance(panelName, panel);
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
                    CloseRegisteredChildren(topName);
                    ActivePanels.Remove(topName);
                    if (!TryCachePanel(topName, top))
                    {
                        ReleasePanelInstance(topName, top);
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
                if (name == keep || IsChildOf(keep, name))
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

                CloseRegisteredChildren(name);
                ActivePanels.Remove(name);
                ReleasePanelInstance(name, panel);
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

                CloseRegisteredChildren(name);
                ActivePanels.Remove(name);
                ReleasePanelInstance(name, panel);
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

        private static DefaultInputActions _uiActions;

        private static void EnsureEventSystem()
        {
            var go = EventSystem.current != null ? EventSystem.current.gameObject : new GameObject("EventSystem");
            if (go.GetComponent<EventSystem>() == null)
            {
                go.AddComponent<EventSystem>();
            }

            var standalone = go.GetComponent<StandaloneInputModule>();
            if (standalone != null)
            {
                UnityEngine.Object.DestroyImmediate(standalone);
            }

            var module = go.GetComponent<InputSystemUIInputModule>();
            if (module == null)
            {
                module = go.AddComponent<InputSystemUIInputModule>();
            }

            BindDefaultUiActions(module);
            go.SetActive(true);
            UnityEngine.Object.DontDestroyOnLoad(go);
        }

        private static void ConfigureInputSystem()
        {
            var settings = InputSystem.settings;
            settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
            settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
#if UNITY_EDITOR
            settings.editorInputBehaviorInPlayMode =
                InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
#endif
            if (settings.supportedDevices.Count > 0)
            {
                settings.supportedDevices = Array.Empty<string>();
            }

            EnsurePointerDevices();
        }

        private static void EnsurePointerDevices()
        {
            foreach (var device in InputSystem.devices)
            {
                if (device is Mouse existing)
                {
                    if (!existing.enabled)
                    {
                        InputSystem.EnableDevice(existing);
                    }

                    return;
                }
            }

            try
            {
                InputSystem.AddDevice<Mouse>();
            }
            catch (Exception e)
            {
                DDoveDebug.LogError(DDoveUIInitInfo.LogTitle, ("mouse", "add failed"), ("error", e.Message));
            }
        }

        private static void BindDefaultUiActions(InputSystemUIInputModule module)
        {
            if (module == null)
            {
                return;
            }

            _uiActions ??= new DefaultInputActions();
            _uiActions.Enable();
            module.actionsAsset = _uiActions.asset;
            module.point = InputActionReference.Create(_uiActions.UI.Point);
            module.leftClick = InputActionReference.Create(_uiActions.UI.Click);
            module.rightClick = InputActionReference.Create(_uiActions.UI.RightClick);
            module.middleClick = InputActionReference.Create(_uiActions.UI.MiddleClick);
            module.scrollWheel = InputActionReference.Create(_uiActions.UI.ScrollWheel);
            module.move = InputActionReference.Create(_uiActions.UI.Navigate);
            module.submit = InputActionReference.Create(_uiActions.UI.Submit);
            module.cancel = InputActionReference.Create(_uiActions.UI.Cancel);
            module.enabled = false;
            module.enabled = true;
        }

        private static bool IsChildOf(string parentName, string childName)
        {
            return ChildOwners.TryGetValue(childName, out var owner) && owner == parentName;
        }

        private static void RegisterChild(string parentName, string childName)
        {
            if (!ChildPanels.TryGetValue(parentName, out var children))
            {
                children = new List<string>();
                ChildPanels[parentName] = children;
            }

            if (!children.Contains(childName))
            {
                children.Add(childName);
            }

            ChildOwners[childName] = parentName;
            PanelsWithChildren.Add(parentName);
        }

        private static void UnregisterChild(string childName)
        {
            if (!ChildOwners.TryGetValue(childName, out var parentName))
            {
                return;
            }

            ChildOwners.Remove(childName);
            if (ChildPanels.TryGetValue(parentName, out var children))
            {
                children.Remove(childName);
            }
        }

        private static void HideSiblingChildren(string parentName, string exceptChild)
        {
            if (!ChildPanels.TryGetValue(parentName, out var children))
            {
                return;
            }

            foreach (var name in children)
            {
                if (name == exceptChild)
                {
                    continue;
                }

                if (ActivePanels.TryGetValue(name, out var sibling))
                {
                    sibling.Hide();
                }
            }
        }

        private static void CloseRegisteredChildren(string parentName)
        {
            if (!ChildPanels.TryGetValue(parentName, out var children))
            {
                return;
            }

            foreach (var childName in children.ToArray())
            {
                CloseRegisteredChildren(childName);
                if (!ActivePanels.TryGetValue(childName, out var child))
                {
                    UnregisterChild(childName);
                    continue;
                }

                child.Hide();
                ActivePanels.Remove(childName);
                RemoveFromPanelStack(childName);
                ReleasePanelInstance(childName, child);
            }
        }

        private static void ReleasePanelInstance(string panelName, IDDoveUIPanel panel)
        {
            UnregisterChild(panelName);
            panel.Close();
            OnPanelClosed(panelName);
            ChildPanels.Remove(panelName);
            PanelsWithChildren.Remove(panelName);
        }

        private static Transform FindDirectContent(Transform parent)
        {
            if (parent == null)
            {
                return null;
            }

            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (child.name == "Content")
                {
                    return child;
                }
            }

            return null;
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
