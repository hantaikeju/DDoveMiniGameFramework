using System;
using DDoveFramework.Editor;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace DDoveFramework.Extension.DDoveUI.Editor
{
    public static class DDoveUITabSampleBuilder
    {
        private const string StageName = "Start";

        [DDoveHotboxEntry("创建样板页内 Tab", "UI 制作", "制作 WndTabDemo 与两个子页，再导出 Prefab")]
        public static void EnsureTabSample()
        {
            if (!BuildParent())
            {
                return;
            }

            if (!BuildChild("WndTabPageA", "Tab A"))
            {
                return;
            }

            if (!BuildChild("WndTabPageB", "Tab B"))
            {
                return;
            }

            AssetDatabase.Refresh();
            if (Application.isBatchMode)
            {
                return;
            }

            EditorUtility.DisplayDialog(
                "DDoveUI",
                "已写入 WndTabDemo、WndTabPageA、WndTabPageB 的制作场景与 Prefab。\n子页父级 Tab 在 Excluded_Top，不进 Prefab。",
                "确定");
        }

        private static bool BuildParent()
        {
            const string panelName = "WndTabDemo";
            var uiRoot = OpenRoot(panelName);
            if (uiRoot == null)
            {
                return false;
            }

            var image = uiRoot.GetComponent<Image>();
            if (image == null)
            {
                image = uiRoot.AddComponent<Image>();
            }

            image.color = new Color(0.12f, 0.16f, 0.22f, 1f);
            RemoveOthers(uiRoot.transform, "BtnBack", "BtnTabA", "BtnTabB", "Content");

            var back = EnsureButton(uiRoot.transform, "BtnBack", "返回", new Vector2(0.06f, 0.9f), new Vector2(0.28f, 0.98f), true);
            var group = EnsureToggleGroup(uiRoot);
            var tabA = EnsureTabToggle(uiRoot.transform, "BtnTabA", "Tab A", new Vector2(0.32f, 0.9f), new Vector2(0.62f, 0.98f), group, true);
            var tabB = EnsureTabToggle(uiRoot.transform, "BtnTabB", "Tab B", new Vector2(0.66f, 0.9f), new Vector2(0.96f, 0.98f), group, false);
            ApplyTabPair(group, tabA.GetComponent<Toggle>(), tabB.GetComponent<Toggle>());
            var content = EnsureContent(uiRoot.transform);
            back.SetSiblingIndex(0);
            tabA.SetSiblingIndex(1);
            tabB.SetSiblingIndex(2);
            content.SetSiblingIndex(3);

            TryAddPanelComponent(uiRoot, "Game.UI.WndTabDemo");
            return Export(uiRoot, panelName);
        }

        private static bool BuildChild(string panelName, string title)
        {
            var uiRoot = OpenRoot(panelName);
            if (uiRoot == null)
            {
                return false;
            }

            var excludedTop = GameObject.Find("Excluded_Top");
            if (excludedTop == null)
            {
                EditorUtility.DisplayDialog("DDoveUI", "制作场景没有 Excluded_Top。", "确定");
                return false;
            }

            StripRootGraphics(uiRoot);
            RemoveOthers(uiRoot.transform, "Title");
            EnsureTitle(uiRoot.transform, title);

            RemoveOthers(excludedTop.transform, "BtnTabA", "BtnTabB");
            var group = EnsureToggleGroup(excludedTop);
            var tabA = EnsureTabToggle(excludedTop.transform, "BtnTabA", "Tab A", new Vector2(0.32f, 0.9f), new Vector2(0.62f, 0.98f), group, true);
            var tabB = EnsureTabToggle(excludedTop.transform, "BtnTabB", "Tab B", new Vector2(0.66f, 0.9f), new Vector2(0.96f, 0.98f), group, false);
            ApplyTabPair(group, tabA.GetComponent<Toggle>(), tabB.GetComponent<Toggle>());
            tabA.SetSiblingIndex(0);
            tabB.SetSiblingIndex(1);

            TryAddPanelComponent(uiRoot, "Game.UI." + panelName);
            return Export(uiRoot, panelName);
        }

        private static GameObject OpenRoot(string panelName)
        {
            if (string.IsNullOrEmpty(DDoveUISceneEditor.EnsureScene(panelName, StageName, DDoveUIPanelKind.Panel)))
            {
                EditorUtility.DisplayDialog("DDoveUI", "创建制作场景失败：\n" + panelName, "确定");
                return null;
            }

            var uiRoot = GameObject.Find("UIRoot");
            if (uiRoot == null)
            {
                EditorUtility.DisplayDialog("DDoveUI", "制作场景没有 UIRoot。", "确定");
                return null;
            }

            return uiRoot;
        }

        private static bool Export(GameObject uiRoot, string panelName)
        {
            if (!DDoveUIPrefabExporter.Save(uiRoot, StageName, panelName))
            {
                EditorUtility.DisplayDialog("DDoveUI", "导出 Prefab 失败：\n" + panelName, "确定");
                return false;
            }

            var scenePath = DDoveUIEditorPaths.GetOrCreateInitInfo().GetCreateScenePath(StageName, panelName);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath);
            return true;
        }

        private static void StripRootGraphics(GameObject uiRoot)
        {
            var image = uiRoot.GetComponent<Image>();
            if (image != null)
            {
                UnityEngine.Object.DestroyImmediate(image);
            }

            if (uiRoot.GetComponent<Graphic>() != null)
            {
                return;
            }

            var renderer = uiRoot.GetComponent<CanvasRenderer>();
            if (renderer != null)
            {
                UnityEngine.Object.DestroyImmediate(renderer);
            }
        }

        private static void RemoveOthers(Transform parent, params string[] keep)
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i);
                if (!Keep(child.name, keep))
                {
                    UnityEngine.Object.DestroyImmediate(child.gameObject);
                }
            }
        }

        private static bool Keep(string name, string[] keep)
        {
            for (var i = 0; i < keep.Length; i++)
            {
                if (keep[i] == name)
                {
                    return true;
                }
            }

            return false;
        }

        private static Transform EnsureContent(Transform parent)
        {
            var existing = parent.Find("Content");
            var go = existing != null
                ? existing.gameObject
                : new GameObject("Content", typeof(RectTransform));
            if (existing == null)
            {
                go.transform.SetParent(parent, false);
            }

            var extras = go.GetComponents<Component>();
            for (var i = extras.Length - 1; i >= 0; i--)
            {
                if (extras[i] is Transform)
                {
                    continue;
                }

                UnityEngine.Object.DestroyImmediate(extras[i]);
            }

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);
            return rect;
        }

        private static void EnsureTitle(Transform parent, string text)
        {
            var existing = parent.Find("Title");
            var go = existing != null
                ? existing.gameObject
                : new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            if (existing == null)
            {
                go.transform.SetParent(parent, false);
            }

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.1f, 0.4f);
            rect.anchorMax = new Vector2(0.9f, 0.6f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);

            var tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 64;
            tmp.color = Color.white;
            tmp.enableWordWrapping = true;
            tmp.overflowMode = TextOverflowModes.Overflow;
            tmp.raycastTarget = true;
            tmp.text = text;
            var font = LoadDefaultFont();
            if (font != null)
            {
                tmp.font = font;
            }

            Bind(go, DDoveUINodeBindType.TextMeshProUGUI, "Title");
        }

        private static ToggleGroup EnsureToggleGroup(GameObject host)
        {
            var group = host.GetComponent<ToggleGroup>();
            if (group == null)
            {
                group = host.AddComponent<ToggleGroup>();
            }

            group.allowSwitchOff = false;
            return group;
        }

        private static Transform EnsureTabToggle(
            Transform parent,
            string name,
            string label,
            Vector2 anchorMin,
            Vector2 anchorMax,
            ToggleGroup group,
            bool isOn)
        {
            var existing = parent.Find(name);
            var go = existing != null
                ? existing.gameObject
                : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Toggle));
            if (existing == null)
            {
                go.transform.SetParent(parent, false);
            }

            var button = go.GetComponent<Button>();
            if (button != null)
            {
                UnityEngine.Object.DestroyImmediate(button);
            }

            if (go.GetComponent<Image>() == null)
            {
                go.AddComponent<Image>();
            }

            var toggle = go.GetComponent<Toggle>();
            if (toggle == null)
            {
                toggle = go.AddComponent<Toggle>();
            }

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);

            var image = go.GetComponent<Image>();
            image.sprite = null;
            image.color = new Color(1f, 1f, 1f, 0f);
            image.raycastTarget = true;

            toggle.targetGraphic = image;
            toggle.graphic = null;
            toggle.transition = Selectable.Transition.None;
            toggle.group = null;
            toggle.isOn = isOn;
            toggle.group = group;

            RemoveOthers(go.transform, "On", "Off");
            var onRoot = EnsureStatePlate(go.transform, "On", Color.white, new Color(0.15f, 0.15f, 0.15f, 1f), label, isOn);
            var offRoot = EnsureStatePlate(go.transform, "Off", new Color(0.55f, 0.55f, 0.55f, 1f), Color.white, label, !isOn);
            TryAddTabOnOff(go, onRoot, offRoot);

            var nodeBind = go.GetComponent<DDoveUINodeBind>();
            if (nodeBind != null)
            {
                UnityEngine.Object.DestroyImmediate(nodeBind);
            }

            return rect;
        }

        private static void ApplyTabPair(ToggleGroup group, Toggle tabOn, Toggle tabOff)
        {
            group.allowSwitchOff = true;
            tabOn.SetIsOnWithoutNotify(false);
            tabOff.SetIsOnWithoutNotify(false);
            tabOn.isOn = true;
            group.allowSwitchOff = false;
        }

        private static Transform EnsureButton(
            Transform parent,
            string name,
            string label,
            Vector2 anchorMin,
            Vector2 anchorMax,
            bool bind)
        {
            var existing = parent.Find(name);
            var go = existing != null
                ? existing.gameObject
                : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            if (existing == null)
            {
                go.transform.SetParent(parent, false);
            }

            if (go.GetComponent<Image>() == null)
            {
                go.AddComponent<Image>();
            }

            if (go.GetComponent<Button>() == null)
            {
                go.AddComponent<Button>();
            }

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);

            var image = go.GetComponent<Image>();
            image.color = new Color(0.2f, 0.45f, 0.85f, 1f);
            image.raycastTarget = true;
            go.GetComponent<Button>().targetGraphic = image;

            EnsureLabel(go.transform, name + "Label", label, Color.white);
            if (bind)
            {
                Bind(go, DDoveUINodeBindType.Button, name);
            }
            else
            {
                var nodeBind = go.GetComponent<DDoveUINodeBind>();
                if (nodeBind != null)
                {
                    UnityEngine.Object.DestroyImmediate(nodeBind);
                }
            }

            return rect;
        }

        private static GameObject EnsureStatePlate(
            Transform parent,
            string name,
            Color plate,
            Color textColor,
            string text,
            bool active)
        {
            var existing = parent.Find(name);
            var go = existing != null
                ? existing.gameObject
                : new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            if (existing == null)
            {
                go.transform.SetParent(parent, false);
            }

            if (go.GetComponent<Image>() == null)
            {
                go.AddComponent<Image>();
            }

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);

            var image = go.GetComponent<Image>();
            image.sprite = null;
            image.color = plate;
            image.raycastTarget = false;

            RemoveOthers(go.transform, "Label");
            EnsureLabel(go.transform, "Label", text, textColor);
            go.SetActive(active);
            return go;
        }

        private static void EnsureLabel(Transform parent, string name, string text, Color color)
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
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);

            var tmp = go.GetComponent<TextMeshProUGUI>();
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 26;
            tmp.color = color;
            tmp.enableWordWrapping = true;
            tmp.overflowMode = TextOverflowModes.Overflow;
            tmp.raycastTarget = false;
            tmp.text = text;
            var font = LoadDefaultFont();
            if (font != null)
            {
                tmp.font = font;
            }
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
            var type = FindType(fullName);
            if (type == null)
            {
                return;
            }

            if (root.GetComponent(type) == null)
            {
                root.AddComponent(type);
            }
        }

        private static void TryAddTabOnOff(GameObject host, GameObject onRoot, GameObject offRoot)
        {
            var type = FindType("Game.Mono.TabOnOff");
            if (type == null)
            {
                return;
            }

            var component = host.GetComponent(type);
            if (component == null)
            {
                component = host.AddComponent(type);
            }

            var so = new SerializedObject(component);
            so.FindProperty("onRoot").objectReferenceValue = onRoot;
            so.FindProperty("offRoot").objectReferenceValue = offRoot;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Type FindType(string fullName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic)
                {
                    continue;
                }

                var type = assembly.GetType(fullName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }
    }
}
