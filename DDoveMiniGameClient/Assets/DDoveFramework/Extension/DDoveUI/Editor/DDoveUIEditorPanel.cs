using DDoveFramework.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DDoveFramework.Extension.DDoveUI.Editor
{
    [DDoveEditorPanel("ui", "UI", 200)]
    public sealed class DDoveUIEditorPanel : IDDoveEditorPanel
    {
        private VisualElement _host;

        public void Build(VisualElement root)
        {
            _host = root;
            Rebuild();
        }

        private void Rebuild()
        {
            if (_host == null)
            {
                return;
            }

            _host.Clear();
            _host.style.minWidth = 0;
            _host.style.flexGrow = 1;
            var info = DDoveUIEditorPaths.GetOrCreateInitInfo();
            _host.Add(BuildRuntimeCard(info));
            _host.Add(BuildPathCard(info));
        }

        private static VisualElement BuildRuntimeCard(DDoveUIInitInfo info)
        {
            var card = Card("显示");
            card.Add(PlateHint("制作场景 CanvasScaler 与运行时共用。默认 1080×1920。", true));

            var plate = new VisualElement();
            plate.AddToClassList("ddove-card-plate");

            var resolution = new Vector2Field("参考分辨率") { value = info.ReferenceResolution };
            resolution.AddToClassList("ddove-field");
            resolution.style.marginTop = 0;
            resolution.RegisterValueChangedCallback(evt => SetVector(info, "_referenceResolution", evt.newValue));
            plate.Add(resolution);

            var match = new Slider("宽高匹配", 0f, 1f)
            {
                value = info.MatchWidthOrHeight,
                showInputField = true
            };
            match.AddToClassList("ddove-field");
            match.RegisterValueChangedCallback(evt => SetFloat(info, "_matchWidthOrHeight", evt.newValue));
            plate.Add(match);

            var cache = new IntegerField("LRU 容量") { value = info.PanelCacheCapacity };
            cache.AddToClassList("ddove-field");
            cache.RegisterValueChangedCallback(evt =>
            {
                var value = Mathf.Max(0, evt.newValue);
                if (value != evt.newValue)
                {
                    cache.SetValueWithoutNotify(value);
                }

                SetInt(info, "_panelCacheCapacity", value);
            });
            plate.Add(cache);
            card.Add(plate);
            return card;
        }

        private static VisualElement BuildPathCard(DDoveUIInitInfo info)
        {
            var card = Card("路径");
            var plate = new VisualElement();
            plate.AddToClassList("ddove-card-plate");
            plate.AddToClassList("ddove-card-plate--lead");
            plate.Add(PathField(info, "制作场景根", info.CreateSceneRoot, "_createSceneRoot", DDoveUIInitInfo.DefaultCreateSceneRoot, true));
            plate.Add(PathField(info, "Prefab 根", info.PrefabRoot, "_prefabRoot", DDoveUIInitInfo.DefaultPrefabRoot, false));
            plate.Add(PathField(info, "绑定代码", info.BindScriptsPath, "_bindScriptsPath", DDoveUIInitInfo.DefaultBindScriptsPath, false));
            plate.Add(PathField(info, "业务代码", info.LogicScriptsPath, "_logicScriptsPath", DDoveUIInitInfo.DefaultLogicScriptsPath, false));
            card.Add(plate);
            return card;
        }

        private static TextField PathField(
            DDoveUIInitInfo info,
            string label,
            string value,
            string property,
            string fallback,
            bool first)
        {
            var field = new TextField(label) { value = value };
            field.AddToClassList("ddove-field");
            if (first)
            {
                field.style.marginTop = 0;
            }

            field.RegisterValueChangedCallback(evt =>
            {
                var next = string.IsNullOrWhiteSpace(evt.newValue) ? fallback : evt.newValue.Trim().Replace("\\", "/");
                if (next != evt.newValue)
                {
                    field.SetValueWithoutNotify(next);
                }

                SetString(info, property, next);
            });
            return field;
        }

        private static VisualElement Card(string title)
        {
            var card = new VisualElement();
            card.AddToClassList("ddove-card");
            var label = new Label(title);
            label.AddToClassList("ddove-card-title");
            card.Add(label);
            return card;
        }

        private static VisualElement PlateHint(string text, bool lead)
        {
            var plate = new VisualElement();
            plate.AddToClassList("ddove-card-plate");
            if (lead)
            {
                plate.AddToClassList("ddove-card-plate--lead");
            }

            var label = new Label(text);
            label.AddToClassList("ddove-hint");
            plate.Add(label);
            return plate;
        }

        private static void SetVector(DDoveUIInitInfo info, string property, Vector2 value)
        {
            var so = new SerializedObject(info);
            so.FindProperty(property).vector2Value = value;
            Apply(so, info);
        }

        private static void SetFloat(DDoveUIInitInfo info, string property, float value)
        {
            var so = new SerializedObject(info);
            so.FindProperty(property).floatValue = value;
            Apply(so, info);
        }

        private static void SetInt(DDoveUIInitInfo info, string property, int value)
        {
            var so = new SerializedObject(info);
            so.FindProperty(property).intValue = value;
            Apply(so, info);
        }

        private static void SetString(DDoveUIInitInfo info, string property, string value)
        {
            var so = new SerializedObject(info);
            so.FindProperty(property).stringValue = value;
            Apply(so, info);
        }

        private static void Apply(SerializedObject so, DDoveUIInitInfo info)
        {
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(info);
            AssetDatabase.SaveAssets();
        }
    }
}
