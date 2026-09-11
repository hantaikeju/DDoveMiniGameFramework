using System;
using DDoveFramework.Editor;
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
            EditorUtility.DisplayDialog(
                "DDoveUI",
                "已写入 WndHome 脚本与 Prefab。若刚生成脚本，等编译后再点一次以挂上组件。\nYoo 收集器需要 Group Start → Assets/GameRes/UI/Start。",
                "确定");
        }

        private static GameObject CreateRoot()
        {
            var root = new GameObject("UIRoot", typeof(RectTransform), typeof(Image));
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            root.GetComponent<Image>().color = new Color(0.12f, 0.16f, 0.22f, 1f);

            var title = new GameObject("Title", typeof(RectTransform), typeof(Text));
            title.transform.SetParent(root.transform, false);
            var titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.1f, 0.45f);
            titleRect.anchorMax = new Vector2(0.9f, 0.55f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            var text = title.GetComponent<Text>();
            text.text = "WndHome";
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 64;
            text.color = Color.white;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            return root;
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
