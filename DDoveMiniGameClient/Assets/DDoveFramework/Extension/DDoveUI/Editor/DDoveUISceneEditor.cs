using System.IO;
using DDoveFramework.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace DDoveFramework.Extension.DDoveUI.Editor
{
    public static class DDoveUISceneEditor
    {
        public static void CreateScene(string panelName, string stageName, DDoveUIPanelKind kind)
        {
            var info = DDoveUIEditorPaths.GetOrCreateInitInfo();
            if (string.IsNullOrWhiteSpace(panelName))
            {
                EditorUtility.DisplayDialog("DDoveUI", "面板名不能为空。", "确定");
                return;
            }

            var scenePath = info.GetCreateScenePath(stageName, panelName);
            if (File.Exists(scenePath))
            {
                EditorUtility.DisplayDialog("DDoveUI", $"场景已存在：\n{scenePath}", "确定");
                return;
            }

            if (string.IsNullOrWhiteSpace(EnsureScene(panelName, stageName, kind)))
            {
                return;
            }

            var root = GameObject.Find(panelName);
            if (root != null)
            {
                Selection.activeGameObject = root;
            }
        }

        public static string EnsureScene(string panelName, string stageName, DDoveUIPanelKind kind)
        {
            var info = DDoveUIEditorPaths.GetOrCreateInitInfo();
            if (string.IsNullOrWhiteSpace(panelName))
            {
                return null;
            }

            var scenePath = info.GetCreateScenePath(stageName, panelName);
            if (File.Exists(scenePath))
            {
                EditorSceneManager.OpenScene(scenePath);
                return scenePath;
            }

            var folder = Path.GetDirectoryName(scenePath)?.Replace("\\", "/");
            DDoveUIEditorPaths.EnsureFolder(folder);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var root = new GameObject(panelName);
            var desc = root.AddComponent<DDoveUIPanelDescription>();
            desc.StageName = string.IsNullOrEmpty(stageName) ? "Start" : stageName;
            desc.PanelKind = kind;
            desc.Namespace = info.NamespaceName;

            var cameraGo = new GameObject("Main Camera", typeof(Camera));
            cameraGo.transform.SetParent(root.transform);
            var camera = cameraGo.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;

            var canvasGo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(root.transform);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = info.ReferenceResolution;
            scaler.matchWidthOrHeight = info.MatchWidthOrHeight;
            scaler.referencePixelsPerUnit = info.ReferencePixelsPerUnit;

            CreateStretch(canvasGo.transform, info.ExcludedBottomName);
            CreateStretch(canvasGo.transform, info.ExportRootName);
            CreateStretch(canvasGo.transform, info.ExcludedTopName);

            var es = new GameObject("EventSystem");
            es.transform.SetParent(root.transform);
            es.AddComponent<EventSystem>();
            es.AddComponent<InputSystemUIInputModule>();

            if (!EditorSceneManager.SaveScene(scene, scenePath))
            {
                return null;
            }

            AssetDatabase.Refresh();
            return scenePath;
        }

        [DDoveHotboxEntry("定位 UI Prefab", "UI 制作", "按当前制作场景定位已导出 Prefab")]
        public static void LocatePrefab()
        {
            var info = DDoveUIEditorPaths.GetOrCreateInitInfo();
            var desc = Object.FindFirstObjectByType<DDoveUIPanelDescription>();
            if (desc == null)
            {
                EditorUtility.DisplayDialog("DDoveUI", "当前场景没有 PanelDescription。", "确定");
                return;
            }

            var path = info.GetPrefabPath(desc.StageName, EditorSceneManager.GetActiveScene().name);
            var prefab = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (prefab == null)
            {
                EditorUtility.DisplayDialog("DDoveUI", $"未找到 Prefab：\n{path}", "确定");
                return;
            }

            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
        }

        [DDoveHotboxEntry("绑定 UI 节点", "UI 制作", "给选中节点加 NodeBind")]
        public static void BindSelectedNodes()
        {
            foreach (var go in Selection.gameObjects)
            {
                if (go.GetComponent<DDoveUINodeBind>() != null)
                {
                    continue;
                }

                var bind = Undo.AddComponent<DDoveUINodeBind>(go);
                bind.ComponentType = Detect(go);
            }
        }

        private static DDoveUINodeBindType Detect(GameObject go)
        {
            if (go.GetComponent<Button>() != null)
            {
                return DDoveUINodeBindType.Button;
            }

            if (go.GetComponent<Image>() != null)
            {
                return DDoveUINodeBindType.Image;
            }

            if (go.GetComponent<TMPro.TextMeshProUGUI>() != null)
            {
                return DDoveUINodeBindType.TextMeshProUGUI;
            }

            return DDoveUINodeBindType.RectTransform;
        }

        private static void CreateStretch(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
        }
    }
}
