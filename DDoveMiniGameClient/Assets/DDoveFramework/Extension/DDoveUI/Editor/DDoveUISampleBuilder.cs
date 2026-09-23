using System;
using System.IO;
using DDoveFramework.Editor;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace DDoveFramework.Extension.DDoveUI.Editor
{
    public static class DDoveUISampleBuilder
    {
        private const string ScenePath = "Assets/GameResExcluded/CreateUIScenes/Start/WndHome.unity";
        private const string PrefabPath = "Assets/GameRes/UI/Start/WndHome.prefab";

        [DDoveHotboxEntry("创建样板 WndHome", "UI 制作", "制作场景摆样板控件，再导出 Prefab")]
        public static void EnsureWndHome()
        {
            var logicPath = "Assets/Game/UI/Start/WndHome.cs";
            if (!File.Exists(logicPath)
                && !DDoveUIPanelCodeGenerator.Generate("WndHome", "Start", DDoveUIPanelKind.Panel, Array.Empty<object>()))
            {
                return;
            }

            if (string.IsNullOrEmpty(DDoveUISceneEditor.EnsureScene("WndHome", "Start", DDoveUIPanelKind.Panel)))
            {
                EditorUtility.DisplayDialog("DDoveUI", "创建 WndHome 制作场景失败。", "确定");
                return;
            }

            var uiRoot = GameObject.Find("UIRoot");
            if (uiRoot == null)
            {
                EditorUtility.DisplayDialog("DDoveUI", "制作场景没有 UIRoot。", "确定");
                return;
            }

            BuildHomeTree(uiRoot);
            TryAddPanelComponent(uiRoot, "Game.UI.WndHome");

            if (!DDoveUIPrefabExporter.Save(uiRoot, "Start", "WndHome"))
            {
                EditorUtility.DisplayDialog("DDoveUI", "导出 Prefab 失败：\n" + PrefabPath, "确定");
                return;
            }

            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ScenePath);
            AssetDatabase.Refresh();
            if (Application.isBatchMode)
            {
                return;
            }

            EditorUtility.DisplayDialog(
                "DDoveUI",
                "已写入制作场景与 Prefab。改布局回场景再导出。\nYoo 收集器需要 Group Start → Assets/GameRes/UI/Start。",
                "确定");
        }

        public static void UpgradeExistingWndHomeTitle()
        {
            UpgradeExistingWndHomeTitleIfLegacy();
        }

        private static void BuildHomeTree(GameObject uiRoot)
        {
            var image = uiRoot.GetComponent<Image>();
            if (image == null)
            {
                image = uiRoot.AddComponent<Image>();
            }

            image.color = new Color(0.12f, 0.16f, 0.22f, 1f);

            EnsureTitle(uiRoot.transform);
            RemoveIfExists(uiRoot.transform, "TxtBag");
            RemoveIfExists(uiRoot.transform, "BtnCollect");
            RemoveIfExists(uiRoot.transform, "ImgLeft");
            RemoveIfExists(uiRoot.transform, "ImgRight");
        }

        private static void RemoveIfExists(Transform parent, string name)
        {
            var existing = parent.Find(name);
            if (existing != null)
            {
                UnityEngine.Object.DestroyImmediate(existing.gameObject);
            }
        }

        private static void EnsureTitle(Transform root)
        {
            var title = root.Find("Title");
            if (title == null)
            {
                var go = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
                title = go.transform;
                title.SetParent(root, false);
                var rect = go.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.1f, 0.82f);
                rect.anchorMax = new Vector2(0.9f, 0.92f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
            }

            UpgradeTitleToTmp(root.gameObject);
            Bind(title.gameObject, DDoveUINodeBindType.TextMeshProUGUI, "Title");
        }

        private static void EnsureLabel(
            Transform parent,
            string name,
            Vector2 anchored,
            Vector2 size,
            int fontSize,
            string text)
        {
            var existing = parent.Find(name);
            var go = existing != null
                ? existing.gameObject
                : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            if (existing == null)
            {
                go.transform.SetParent(parent, false);
            }

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchored;
            rect.sizeDelta = size;

            var tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = fontSize;
            tmp.color = Color.white;
            tmp.enableWordWrapping = true;
            tmp.overflowMode = TextOverflowModes.Overflow;
            if (!string.IsNullOrEmpty(text))
            {
                tmp.text = text;
            }

            var font = LoadDefaultFont();
            if (font != null)
            {
                tmp.font = font;
            }

            Bind(go, DDoveUINodeBindType.TextMeshProUGUI, name);
        }

        private static void EnsureButton(Transform parent, string name, string label, Vector2 anchored, Vector2 size)
        {
            var existing = parent.Find(name);
            var go = existing != null
                ? existing.gameObject
                : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            if (existing == null)
            {
                go.transform.SetParent(parent, false);
            }

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchored;
            rect.sizeDelta = size;
            go.GetComponent<Image>().color = new Color(0.2f, 0.45f, 0.85f, 1f);

            EnsureLabel(go.transform, name + "Label", Vector2.zero, size, 26, label);
            Bind(go, DDoveUINodeBindType.Button, name);
        }

        private static void EnsureImage(Transform parent, string name, Vector2 anchored, Vector2 size)
        {
            var existing = parent.Find(name);
            var go = existing != null
                ? existing.gameObject
                : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            if (existing == null)
            {
                go.transform.SetParent(parent, false);
            }

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchored;
            rect.sizeDelta = size;
            var image = go.GetComponent<Image>();
            image.preserveAspect = true;
            Bind(go, DDoveUINodeBindType.Image, name);
        }

        private static void Bind(GameObject go, DDoveUINodeBindType type, string memberName)
        {
            var bind = go.GetComponent<DDoveUINodeBind>();
            if (bind == null)
            {
                bind = go.AddComponent<DDoveUINodeBind>();
            }

            bind.ComponentType = type;
            bind.MemberName = memberName;
        }

        private static void UpgradeExistingWndHomeTitleIfLegacy()
        {
            var info = DDoveUIEditorPaths.GetOrCreateInitInfo();
            var prefabPath = info.GetPrefabPath("Start", "WndHome");
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (existing == null)
            {
                return;
            }

            var title = existing.transform.Find("Title");
            if (title == null || title.GetComponent<Text>() == null)
            {
                return;
            }

            var root = PrefabUtility.LoadPrefabContents(prefabPath);
            try
            {
                UpgradeTitleToTmp(root);
                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }

            AssetDatabase.Refresh();
        }

        private static void UpgradeTitleToTmp(GameObject root)
        {
            var title = root.transform.Find("Title");
            if (title == null)
            {
                return;
            }

            if (title.GetComponent<TextMeshProUGUI>() != null)
            {
                var leftover = title.GetComponent<Text>();
                if (leftover != null)
                {
                    UnityEngine.Object.DestroyImmediate(leftover);
                }

                var tmp = title.GetComponent<TextMeshProUGUI>();
                if (tmp.font == null)
                {
                    tmp.font = LoadDefaultFont();
                }

                return;
            }

            var old = title.GetComponent<Text>();
            if (old == null)
            {
                return;
            }

            var str = old.text;
            UnityEngine.Object.DestroyImmediate(old);
            var created = title.gameObject.AddComponent<TextMeshProUGUI>();
            ApplyTitleStyle(created, str);
        }

        private static void ApplyTitleStyle(TextMeshProUGUI text, string content)
        {
            text.text = content;
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 64;
            text.color = Color.white;
            var font = LoadDefaultFont();
            if (font != null)
            {
                text.font = font;
            }
        }

        private static TMP_FontAsset LoadDefaultFont()
        {
            if (TMP_Settings.defaultFontAsset != null)
            {
                return TMP_Settings.defaultFontAsset;
            }

            var fromResources = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (fromResources != null)
            {
                return fromResources;
            }

            const string packagePath =
                "Packages/com.unity.textmeshpro/Package Resources/Fonts & Materials/LiberationSans SDF.asset";
            return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(packagePath);
        }

        private static void TryAddPanelComponent(GameObject root, string fullName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic)
                {
                    continue;
                }

                var type = assembly.GetType(fullName);
                if (type == null)
                {
                    continue;
                }

                if (root.GetComponent(type) == null)
                {
                    root.AddComponent(type);
                }

                return;
            }
        }
    }
}
