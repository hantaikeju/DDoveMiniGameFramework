using UnityEditor;
using UnityEngine.UIElements;

namespace DDoveFramework.Editor
{
    [DDoveEditorPanel("architecture", "Architecture", 150)]
    public sealed class DDoveArchitectureEditorPanel : IDDoveEditorPanel
    {
        public void Build(VisualElement root)
        {
            root.style.minWidth = 0;
            root.style.flexGrow = 1;
            root.Add(BuildPathCard(DDoveArchitectureBindConfig.GetOrCreate()));
        }

        private static VisualElement BuildPathCard(DDoveArchitectureBindConfig config)
        {
            var card = new VisualElement();
            card.AddToClassList("ddove-card");
            var heading = new Label("路径");
            heading.AddToClassList("ddove-card-title");
            card.Add(heading);

            var plate = new VisualElement();
            plate.AddToClassList("ddove-card-plate");
            plate.AddToClassList("ddove-card-plate--lead");
            plate.Add(PathField(config, "Model 路径", config.ModelPath, "_modelPath",
                DDoveArchitectureBindConfig.DefaultModelPath, true));
            plate.Add(PathField(config, "System 路径", config.SystemPath, "_systemPath",
                DDoveArchitectureBindConfig.DefaultSystemPath, false));
            plate.Add(PathField(config, "Utility 路径", config.UtilityPath, "_utilityPath",
                DDoveArchitectureBindConfig.DefaultUtilityPath, false));
            card.Add(plate);
            return card;
        }

        private static TextField PathField(
            DDoveArchitectureBindConfig config,
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
                var next = string.IsNullOrWhiteSpace(evt.newValue)
                    ? fallback
                    : evt.newValue.Trim().Replace("\\", "/");
                if (next != evt.newValue)
                {
                    field.SetValueWithoutNotify(next);
                }

                var so = new SerializedObject(config);
                so.FindProperty(property).stringValue = next;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();
            });
            return field;
        }
    }
}
