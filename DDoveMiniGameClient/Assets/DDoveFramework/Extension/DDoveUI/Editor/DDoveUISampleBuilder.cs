using System;
using DDoveFramework.Editor;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DDoveFramework.Extension.DDoveUI.Editor
{
    public static class DDoveUISampleBuilder
    {
        [DDoveHotboxEntry("创建样板 WndHome", "UI 制作", "生成样板绑定、业务脚本和 Prefab")]
        public static void EnsureWndHome()
        {
            var info = DDoveUIEditorPaths.GetOrCreateInitInfo();
            if (!DDoveUIPanelCodeGenerator.Generate("WndHome", "Start", DDoveUIPanelKind.Panel, Array.Empty<object>()))
            {
                return;
            }

            DDoveUIEditorPaths.EnsureFolder("Assets/GameRes/UI/Start");
            var prefabPath = info.GetPrefabPath("Start", "WndHome");
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            var root = existing != null
                ? PrefabUtility.LoadPrefabContents(prefabPath)
                : CreateRoot();

            TryAddPanelComponent(root, "Game.UI.WndHome");
            UpgradeTitleToTmp(root);
            if (existing != null)
            {
                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                PrefabUtility.UnloadPrefabContents(root);
            }
            else
            {
                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                UnityEngine.Object.DestroyImmediate(root);
            }

            AssetDatabase.Refresh();
            if (Application.isBatchMode)
            {
                return;
            }

            EditorUtility.DisplayDialog(
                "DDoveUI",
                "已写入 WndHome 脚本与 Prefab。若刚生成脚本，等编译后再点一次以挂上组件。\nYoo 收集器需要 Group Start → Assets/GameRes/UI/Start。",
                "确定");
        }

        public static void UpgradeExistingWndHomeTitle()
        {
            UpgradeExistingWndHomeTitleIfLegacy();
        }

        private static GameObject CreateRoot()
        {
            var root = new GameObject("UIRoot", typeof(RectTransform), typeof(Image));
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            root.GetComponent<Image>().color = new Color(0.12f, 0.16f, 0.22f, 1f);

            var title = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            title.transform.SetParent(root.transform, false);
            var titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.1f, 0.45f);
            titleRect.anchorMax = new Vector2(0.9f, 0.55f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            ApplyTitleStyle(title.GetComponent<TextMeshProUGUI>(), "WndHome");
            return root;
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
