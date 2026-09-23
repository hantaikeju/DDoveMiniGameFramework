using Cysharp.Threading.Tasks;
using DDoveFramework.Extension.DDoveUI;
using DDoveFramework.Extension.DDoveUI.Editor;
using Game.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Editor
{
    public static class OpenCollectDemoMenu
    {
        const string ScenePath = "Assets/GameResExcluded/CreateUIScenes/Start/WndCollectDemo.unity";
        const string PrefabPath = "Assets/GameRes/UI/Start/WndCollectDemo.prefab";

        [MenuItem("Game/Open WndCollectDemo")]
        public static void Open()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[WndCollectDemo] open in Play Mode.");
                return;
            }

            DDoveUIKit.OpenAsync<WndCollectDemo>().Forget();
        }

        [MenuItem("Game/Build WndCollectDemo Assets")]
        public static void BuildAssets()
        {
            if (string.IsNullOrEmpty(DDoveUISceneEditor.EnsureScene("WndCollectDemo", "Start", DDoveUIPanelKind.Panel)))
            {
                Debug.LogError("[WndCollectDemo] ensure scene failed.");
                return;
            }

            var uiRoot = GameObject.Find("UIRoot");
            if (uiRoot == null)
            {
                Debug.LogError("[WndCollectDemo] missing UIRoot.");
                return;
            }

            BuildCollectTree(uiRoot);

            if (uiRoot.GetComponent<WndCollectDemo>() == null)
            {
                uiRoot.AddComponent<WndCollectDemo>();
            }

            if (!DDoveUIPrefabExporter.Save(uiRoot, "Start", "WndCollectDemo"))
            {
                Debug.LogError("[WndCollectDemo] export prefab failed: " + PrefabPath);
                return;
            }

            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ScenePath);
            AssetDatabase.Refresh();
            Debug.Log("[WndCollectDemo] built " + ScenePath + " and " + PrefabPath);
        }

        static void BuildCollectTree(GameObject uiRoot)
        {
            var image = uiRoot.GetComponent<Image>();
            if (image == null)
            {
                image = uiRoot.AddComponent<Image>();
            }

            image.color = new Color(0.12f, 0.16f, 0.22f, 1f);

            EnsureTitle(uiRoot.transform);
            EnsureLabel(uiRoot.transform, "TxtBag", new Vector2(0f, 260f), new Vector2(480f, 48f), 28, string.Empty);
            EnsureButton(uiRoot.transform, "BtnCollect", "领取", new Vector2(0f, 200f), new Vector2(200f, 56f));
            EnsureImage(uiRoot.transform, "ImgLeft", new Vector2(-180f, 0f), new Vector2(160f, 160f));
            EnsureImage(uiRoot.transform, "ImgRight", new Vector2(180f, 0f), new Vector2(160f, 160f));
            EnsureBackButton(uiRoot.transform);
        }

        static void EnsureTitle(Transform root)
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

            var tmp = title.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.fontSize = 64;
                tmp.color = Color.white;
                if (string.IsNullOrEmpty(tmp.text))
                {
                    tmp.text = "WndCollectDemo";
                }

                var font = LoadDefaultFont();
                if (font != null)
                {
                    tmp.font = font;
                }
            }

            Bind(title.gameObject, DDoveUINodeBindType.TextMeshProUGUI, "Title");
        }

        static void EnsureLabel(
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

        static void EnsureButton(Transform parent, string name, string label, Vector2 anchored, Vector2 size)
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

        static void EnsureBackButton(Transform parent)
        {
            var existing = parent.Find("BtnBack");
            var go = existing != null
                ? existing.gameObject
                : new GameObject("BtnBack", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            if (existing == null)
            {
                go.transform.SetParent(parent, false);
            }

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.04f, 0.93f);
            rect.anchorMax = new Vector2(0.28f, 0.98f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
            go.GetComponent<Image>().color = new Color(0.2f, 0.45f, 0.85f, 1f);

            EnsureLabel(go.transform, "BtnBackLabel", Vector2.zero, new Vector2(200f, 56f), 26, "返回");
            var label = go.transform.Find("BtnBackLabel");
            if (label != null)
            {
                var labelRect = label.GetComponent<RectTransform>();
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.one;
                labelRect.offsetMin = Vector2.zero;
                labelRect.offsetMax = Vector2.zero;
                labelRect.anchoredPosition = Vector2.zero;
                labelRect.sizeDelta = Vector2.zero;
            }

            Bind(go, DDoveUINodeBindType.Button, "BtnBack");
        }

        static void EnsureImage(Transform parent, string name, Vector2 anchored, Vector2 size)
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

        static void Bind(GameObject go, DDoveUINodeBindType type, string memberName)
        {
            var bind = go.GetComponent<DDoveUINodeBind>();
            if (bind == null)
            {
                bind = go.AddComponent<DDoveUINodeBind>();
            }

            bind.ComponentType = type;
            bind.MemberName = memberName;
        }

        static TMP_FontAsset LoadDefaultFont()
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
    }
}
