using Cysharp.Threading.Tasks;
using DDoveFramework.Extension.DDoveUI;
using DDoveFramework.Extension.DDoveUI.Editor;
using Game.Mono;
using Game.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Editor
{
    public static class OpenHomeEntryMenu
    {
        const string ScenePath = "Assets/GameResExcluded/CreateUIScenes/Start/WndHome.unity";
        const string PrefabPath = "Assets/GameRes/UI/Start/WndHome.prefab";

        [MenuItem("Game/Open WndHome")]
        public static void Open()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[WndHome] open in Play Mode.");
                return;
            }

            DDoveUIKit.NavigateToAsync<WndHome>().Forget();
        }

        [MenuItem("Game/Build WndHome Assets")]
        public static void BuildAssets()
        {
            if (string.IsNullOrEmpty(DDoveUISceneEditor.EnsureScene("WndHome", "Start", DDoveUIPanelKind.Panel)))
            {
                Debug.LogError("[WndHome] ensure scene failed.");
                return;
            }

            var uiRoot = GameObject.Find("UIRoot");
            if (uiRoot == null)
            {
                Debug.LogError("[WndHome] missing UIRoot.");
                return;
            }

            RemoveCollectTree(uiRoot.transform);
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(uiRoot);
            foreach (var t in uiRoot.GetComponentsInChildren<Transform>(true))
            {
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
            }

            if (uiRoot.GetComponentInChildren<LoopList>(true) == null)
            {
                BuildListTree(uiRoot);
            }

            if (uiRoot.GetComponent<WndHome>() == null)
            {
                uiRoot.AddComponent<WndHome>();
            }

            if (!DDoveUIPrefabExporter.Save(uiRoot, "Start", "WndHome"))
            {
                Debug.LogError("[WndHome] export prefab failed: " + PrefabPath);
                return;
            }

            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ScenePath);
            AssetDatabase.Refresh();
            Debug.Log("[WndHome] built " + ScenePath + " and " + PrefabPath);
        }

        static void RemoveCollectTree(Transform root)
        {
            RemoveIfExists(root, "TxtBag");
            RemoveIfExists(root, "BtnCollect");
            RemoveIfExists(root, "ImgLeft");
            RemoveIfExists(root, "ImgRight");
        }

        static void RemoveIfExists(Transform parent, string name)
        {
            var existing = parent.Find(name);
            if (existing != null)
            {
                Object.DestroyImmediate(existing.gameObject);
            }
        }

        static void BuildListTree(GameObject uiRoot)
        {
            var image = uiRoot.GetComponent<Image>();
            if (image == null)
            {
                image = uiRoot.AddComponent<Image>();
            }

            image.color = new Color(0.12f, 0.16f, 0.22f, 1f);

            var listGo = new GameObject("LoopList", typeof(RectTransform), typeof(Image), typeof(ScrollRect));
            var listRect = listGo.GetComponent<RectTransform>();
            listRect.SetParent(uiRoot.transform, false);
            Stretch(listRect, new Vector2(0.06f, 0.1f), new Vector2(0.94f, 0.88f));
            listGo.GetComponent<Image>().color = new Color(0.08f, 0.1f, 0.14f, 0.4f);

            var bind = listGo.AddComponent<DDoveUINodeBind>();
            bind.ComponentType = DDoveUINodeBindType.RectTransform;
            bind.MemberName = "ListRoot";

            var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(Image), typeof(Mask));
            var viewportRect = viewport.GetComponent<RectTransform>();
            viewportRect.SetParent(listRect, false);
            Stretch(viewportRect, Vector2.zero, Vector2.one);
            viewport.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.01f);
            viewport.GetComponent<Mask>().showMaskGraphic = false;

            var content = new GameObject("Content", typeof(RectTransform));
            var contentRect = content.GetComponent<RectTransform>();
            contentRect.SetParent(viewportRect, false);
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0f, 0f);

            var template = new GameObject("itemTemplate", typeof(RectTransform), typeof(Image), typeof(Button));
            var templateRect = template.GetComponent<RectTransform>();
            templateRect.SetParent(contentRect, false);
            templateRect.anchorMin = new Vector2(0f, 1f);
            templateRect.anchorMax = new Vector2(1f, 1f);
            templateRect.pivot = new Vector2(0.5f, 1f);
            templateRect.sizeDelta = new Vector2(0f, 100f);
            templateRect.anchoredPosition = Vector2.zero;
            template.GetComponent<Image>().color = new Color(0.18f, 0.28f, 0.42f, 1f);
            template.GetComponent<Button>().navigation = new Navigation { mode = Navigation.Mode.None };
            template.AddComponent<HomeEntryRow>();
            template.SetActive(false);

            var label = new GameObject("ItemLabel", typeof(RectTransform), typeof(TextMeshProUGUI));
            var labelRect = label.GetComponent<RectTransform>();
            labelRect.SetParent(templateRect, false);
            Stretch(labelRect, Vector2.zero, Vector2.one);
            var tmp = label.GetComponent<TextMeshProUGUI>();
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 36;
            tmp.color = Color.white;
            tmp.text = "0";
            var font = TMP_Settings.defaultFontAsset
                ?? Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (font != null)
            {
                tmp.font = font;
            }

            var scroll = listGo.GetComponent<ScrollRect>();
            scroll.content = contentRect;
            scroll.viewport = viewportRect;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            scroll.inertia = true;

            var loop = listGo.AddComponent<LoopList>();
            var so = new SerializedObject(loop);
            so.FindProperty("itemTemplate").objectReferenceValue = templateRect;
            so.FindProperty("itemSize").floatValue = 100f;
            so.FindProperty("spacing").floatValue = 8f;
            so.FindProperty("previewCount").intValue = 6;
            var padding = so.FindProperty("padding");
            padding.FindPropertyRelative("m_Left").intValue = 8;
            padding.FindPropertyRelative("m_Right").intValue = 8;
            padding.FindPropertyRelative("m_Top").intValue = 8;
            padding.FindPropertyRelative("m_Bottom").intValue = 8;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        static void Stretch(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
        }
    }
}
