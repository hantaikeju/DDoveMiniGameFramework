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
    public static class OpenLoopDemoMenu
    {
        const string ScenePath = "Assets/GameResExcluded/CreateUIScenes/Start/WndLoopDemo.unity";
        const string PrefabPath = "Assets/GameRes/UI/Start/WndLoopDemo.prefab";
        const int DemoCount = 80;

        [MenuItem("Game/Open WndLoopDemo")]
        public static void Open()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[WndLoopDemo] open in Play Mode.");
                return;
            }

            DDoveUIKit.OpenAsync<WndLoopDemo>().Forget();
        }

        [MenuItem("Game/Build WndLoopDemo Assets")]
        public static void BuildAssets()
        {
            if (string.IsNullOrEmpty(DDoveUISceneEditor.EnsureScene("WndLoopDemo", "Start", DDoveUIPanelKind.Panel)))
            {
                Debug.LogError("[WndLoopDemo] ensure scene failed.");
                return;
            }

            var uiRoot = GameObject.Find("UIRoot");
            if (uiRoot == null)
            {
                Debug.LogError("[WndLoopDemo] missing UIRoot.");
                return;
            }

            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(uiRoot);
            foreach (var t in uiRoot.GetComponentsInChildren<Transform>(true))
            {
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
            }

            if (uiRoot.GetComponentInChildren<LoopList>(true) == null)
            {
                BuildListTree(uiRoot);
            }

            EnsureBtnBack(uiRoot);

            if (uiRoot.GetComponent<WndLoopDemo>() == null)
            {
                uiRoot.AddComponent<WndLoopDemo>();
            }

            if (!DDoveUIPrefabExporter.Save(uiRoot, "Start", "WndLoopDemo"))
            {
                Debug.LogError("[WndLoopDemo] export prefab failed: " + PrefabPath);
                return;
            }

            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ScenePath);
            AssetDatabase.Refresh();
            Debug.Log("[WndLoopDemo] built " + ScenePath + " and " + PrefabPath + ", SetCount " + DemoCount);
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

            var template = new GameObject("itemTemplate", typeof(RectTransform), typeof(Button));
            var templateRect = template.GetComponent<RectTransform>();
            templateRect.SetParent(contentRect, false);
            templateRect.anchorMin = new Vector2(0f, 1f);
            templateRect.anchorMax = new Vector2(1f, 1f);
            templateRect.pivot = new Vector2(0.5f, 1f);
            templateRect.sizeDelta = new Vector2(0f, 100f);
            templateRect.anchoredPosition = Vector2.zero;
            templateRect.localScale = Vector3.one;
            var button = template.GetComponent<Button>();
            button.navigation = new Navigation { mode = Navigation.Mode.None };

            var anim = new GameObject("Anim", typeof(RectTransform), typeof(CanvasGroup));
            var animRect = anim.GetComponent<RectTransform>();
            animRect.SetParent(templateRect, false);
            Stretch(animRect, Vector2.zero, Vector2.one);
            var animImage = anim.AddComponent<Image>();
            animImage.color = new Color(0.18f, 0.28f, 0.42f, 1f);
            button.targetGraphic = animImage;

            template.AddComponent<DemoRow>();
            template.AddComponent<DefaultTween_V>();
            template.SetActive(false);

            var label = new GameObject("ItemLabel", typeof(RectTransform), typeof(TextMeshProUGUI));
            var labelRect = label.GetComponent<RectTransform>();
            labelRect.SetParent(animRect, false);
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

        static void EnsureBtnBack(GameObject uiRoot)
        {
            var existing = uiRoot.transform.Find("BtnBack");
            if (existing != null)
            {
                return;
            }

            var go = new GameObject("BtnBack", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(uiRoot.transform, false);
            Stretch(rect, new Vector2(0.06f, 0.9f), new Vector2(0.28f, 0.98f));
            go.GetComponent<Image>().color = new Color(0.2f, 0.45f, 0.85f, 1f);

            var bind = go.AddComponent<DDoveUINodeBind>();
            bind.ComponentType = DDoveUINodeBindType.Button;
            bind.MemberName = "BtnBack";

            var label = new GameObject("BtnBackLabel", typeof(RectTransform), typeof(TextMeshProUGUI));
            var labelRect = label.GetComponent<RectTransform>();
            labelRect.SetParent(rect, false);
            Stretch(labelRect, Vector2.zero, Vector2.one);
            var tmp = label.GetComponent<TextMeshProUGUI>();
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 26;
            tmp.color = Color.white;
            tmp.text = "返回";
            var font = TMP_Settings.defaultFontAsset
                ?? Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (font != null)
            {
                tmp.font = font;
            }
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
