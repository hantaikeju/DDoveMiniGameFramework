using DDoveFramework.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DDoveFramework.Extension.DDoveUI.Editor
{
    public sealed class DDoveUISceneCreateWindow : EditorWindow
    {
        private string _panelName = "WndHome";
        private string _stageName = "Start";
        private DDoveUIPanelKind _kind = DDoveUIPanelKind.Panel;

        [DDoveHotboxEntry("创建 UI 场景", "UI 制作", "按阶段建空 UI 制作场景")]
        public static void Open()
        {
            var window = CreateInstance<DDoveUISceneCreateWindow>();
            window.titleContent = new GUIContent("创建 UI 场景");
            window.minSize = new Vector2(360f, 200f);
            window.maxSize = new Vector2(360f, 200f);
            window.position = DDoveEditorUi.Centered(360f, 200f);
            window.ShowUtility();
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            DDoveEditorUi.ApplySheet(root);
            root.style.paddingLeft = 12;
            root.style.paddingRight = 12;
            root.style.paddingTop = 10;
            root.style.paddingBottom = 10;

            var panel = new TextField("面板名") { value = _panelName };
            var stage = new TextField("阶段") { value = _stageName };
            var kind = new EnumField("类型", _kind);
            panel.AddToClassList("ddove-field");
            stage.AddToClassList("ddove-field");
            kind.AddToClassList("ddove-field");
            panel.RegisterValueChangedCallback(evt => _panelName = evt.newValue);
            stage.RegisterValueChangedCallback(evt => _stageName = evt.newValue);
            kind.RegisterValueChangedCallback(evt => _kind = (DDoveUIPanelKind)evt.newValue);
            root.Add(panel);
            root.Add(stage);
            root.Add(kind);

            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.FlexEnd;
            row.style.marginTop = 12;
            var cancel = new Button(Close) { text = "取消" };
            cancel.style.width = 72;
            var create = new Button(() =>
            {
                DDoveUISceneEditor.CreateScene(_panelName.Trim(), _stageName.Trim(), _kind);
                Close();
            })
            {
                text = "创建"
            };
            create.style.width = 72;
            row.Add(cancel);
            row.Add(create);
            root.Add(row);
        }
    }
}
