using DDoveFramework.Editor;
using Game.Mono;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Game.Editor
{
    public sealed class CreateLoopListWindow : EditorWindow
    {
        LoopDirection _direction = LoopDirection.Vertical;
        float _itemSize = 100f;
        float _spacing = 8f;
        int _padding = 8;

        [DDoveHotboxEntry("创建 LoopList", "UI 制作", "在 UIRoot 下挂一条空 LoopList，创建后自己拖位置")]
        public static void Open()
        {
            var window = GetWindow<CreateLoopListWindow>();
            window.titleContent = new GUIContent("创建 LoopList");
            window.minSize = new Vector2(420f, 268f);
            window.maxSize = new Vector2(420f, 268f);
            window.position = DDoveEditorUi.Centered(420f, 268f);
        }

        void CreateGUI()
        {
            var root = rootVisualElement;
            DDoveEditorUi.ApplySheet(root);
            root.AddToClassList("ddove-root");
            root.style.paddingLeft = 16;
            root.style.paddingRight = 16;
            root.style.paddingTop = 16;
            root.style.paddingBottom = 16;

            var card = new VisualElement();
            card.AddToClassList("ddove-card");
            card.style.marginBottom = 0;
            var heading = new Label("列表");
            heading.AddToClassList("ddove-card-title");
            card.Add(heading);

            var plate = new VisualElement();
            plate.AddToClassList("ddove-card-plate");
            plate.AddToClassList("ddove-card-plate--lead");
            var direction = new EnumField("方向", _direction);
            var itemSize = new FloatField("条尺寸") { value = _itemSize };
            var spacing = new FloatField("间距") { value = _spacing };
            var padding = new IntegerField("边距") { value = _padding };
            direction.AddToClassList("ddove-field");
            itemSize.AddToClassList("ddove-field");
            spacing.AddToClassList("ddove-field");
            padding.AddToClassList("ddove-field");
            direction.style.marginTop = 0;
            direction.RegisterValueChangedCallback(evt => _direction = (LoopDirection)evt.newValue);
            itemSize.RegisterValueChangedCallback(evt => _itemSize = evt.newValue);
            spacing.RegisterValueChangedCallback(evt => _spacing = evt.newValue);
            padding.RegisterValueChangedCallback(evt => _padding = evt.newValue);
            plate.Add(direction);
            plate.Add(itemSize);
            plate.Add(spacing);
            plate.Add(padding);
            card.Add(plate);
            root.Add(card);

            var actions = new VisualElement();
            actions.AddToClassList("ddove-actions");
            var create = new UnityEngine.UIElements.Button(() =>
            {
                if (TryCreate(_direction, _itemSize, _spacing, _padding))
                {
                    Close();
                }
            })
            {
                text = "创建"
            };
            actions.Add(create);
            root.Add(actions);
        }

        static bool TryCreate(LoopDirection direction, float itemSize, float spacing, int padding)
        {
            if (!TryResolveParent(out var parent, out var message))
            {
                Debug.LogError("[创建 LoopList] " + message);
                EditorUtility.DisplayDialog("创建 LoopList", message, "确定");
                return false;
            }

            var listGo = BuildList(parent, direction, itemSize, spacing, padding);
            Undo.RegisterCreatedObjectUndo(listGo, "创建 LoopList");
            Selection.activeGameObject = listGo;
            EditorSceneManager.MarkSceneDirty(parent.gameObject.scene);
            return true;
        }

        static bool TryResolveParent(out Transform parent, out string message)
        {
            parent = null;
            message = null;
            var uiRoot = FindUiRoot();
            if (uiRoot == null)
            {
                message = "场景里没有名为 UIRoot 的节点，未创建 LoopList。";
                return false;
            }

            parent = uiRoot.transform;
            return true;
        }

        static GameObject FindUiRoot()
        {
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null && stage.prefabContentsRoot != null)
            {
                return FindInRoot(stage.prefabContentsRoot);
            }

            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                if (!scene.isLoaded)
                {
                    continue;
                }

                foreach (var root in scene.GetRootGameObjects())
                {
                    var found = FindInRoot(root);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }

            return null;
        }

        static GameObject FindInRoot(GameObject root)
        {
            if (root.name == "UIRoot")
            {
                return root;
            }

            var found = FindNamed(root.transform, "UIRoot");
            return found != null ? found.gameObject : null;
        }

        static Transform FindNamed(Transform node, string name)
        {
            for (int i = 0; i < node.childCount; i++)
            {
                var child = node.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }

                var nested = FindNamed(child, name);
                if (nested != null)
                {
                    return nested;
                }
            }

            return null;
        }

        static GameObject BuildList(Transform parent, LoopDirection direction, float itemSize, float spacing, int padding)
        {
            bool horizontal = direction == LoopDirection.Horizontal;
            var listGo = new GameObject(NextName(parent, "LoopList"), typeof(RectTransform), typeof(UnityEngine.UI.Image), typeof(ScrollRect));
            var listRect = listGo.GetComponent<RectTransform>();
            listRect.SetParent(parent, false);
            Stretch(listRect, Vector2.zero, Vector2.one);
            listGo.GetComponent<UnityEngine.UI.Image>().color = new Color(0.08f, 0.1f, 0.14f, 0.4f);

            var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(UnityEngine.UI.Image), typeof(Mask));
            var viewportRect = viewport.GetComponent<RectTransform>();
            viewportRect.SetParent(listRect, false);
            Stretch(viewportRect, Vector2.zero, Vector2.one);
            viewport.GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 1f, 1f, 0.01f);
            viewport.GetComponent<Mask>().showMaskGraphic = false;

            var content = new GameObject("Content", typeof(RectTransform));
            var contentRect = content.GetComponent<RectTransform>();
            contentRect.SetParent(viewportRect, false);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = Vector2.zero;
            if (horizontal)
            {
                contentRect.anchorMin = new Vector2(0f, 0f);
                contentRect.anchorMax = new Vector2(0f, 1f);
                contentRect.pivot = new Vector2(0f, 0.5f);
            }
            else
            {
                contentRect.anchorMin = new Vector2(0f, 1f);
                contentRect.anchorMax = new Vector2(1f, 1f);
                contentRect.pivot = new Vector2(0.5f, 1f);
            }

            var template = new GameObject("itemTemplate", typeof(RectTransform));
            var templateRect = template.GetComponent<RectTransform>();
            templateRect.SetParent(contentRect, false);
            templateRect.localScale = Vector3.one;
            int pad = padding;
            if (horizontal)
            {
                templateRect.anchorMin = new Vector2(0f, 0f);
                templateRect.anchorMax = new Vector2(0f, 1f);
                templateRect.pivot = new Vector2(0f, 0.5f);
                templateRect.sizeDelta = new Vector2(itemSize, -(pad + pad));
                templateRect.anchoredPosition = new Vector2(pad, 0f);
            }
            else
            {
                templateRect.anchorMin = new Vector2(0f, 1f);
                templateRect.anchorMax = new Vector2(1f, 1f);
                templateRect.pivot = new Vector2(0.5f, 1f);
                templateRect.sizeDelta = new Vector2(-(pad + pad), itemSize);
                templateRect.anchoredPosition = new Vector2(0f, -pad);
            }

            template.SetActive(false);

            var scroll = listGo.GetComponent<ScrollRect>();
            scroll.content = contentRect;
            scroll.viewport = viewportRect;
            scroll.horizontal = horizontal;
            scroll.vertical = !horizontal;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.inertia = true;

            var loop = listGo.AddComponent<LoopList>();
            var so = new SerializedObject(loop);
            so.FindProperty("itemTemplate").objectReferenceValue = templateRect;
            so.FindProperty("direction").enumValueIndex = (int)direction;
            so.FindProperty("itemSize").floatValue = itemSize;
            so.FindProperty("spacing").floatValue = spacing;
            var padProp = so.FindProperty("padding");
            padProp.FindPropertyRelative("m_Left").intValue = pad;
            padProp.FindPropertyRelative("m_Right").intValue = pad;
            padProp.FindPropertyRelative("m_Top").intValue = pad;
            padProp.FindPropertyRelative("m_Bottom").intValue = pad;
            so.ApplyModifiedPropertiesWithoutUndo();
            return listGo;
        }

        static string NextName(Transform parent, string baseName)
        {
            if (parent.Find(baseName) == null)
            {
                return baseName;
            }

            int index = 1;
            while (parent.Find(baseName + " (" + index + ")") != null)
            {
                index++;
            }

            return baseName + " (" + index + ")";
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
