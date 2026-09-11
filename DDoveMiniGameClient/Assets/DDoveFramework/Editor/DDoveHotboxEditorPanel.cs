using UnityEngine.UIElements;

namespace DDoveFramework.Editor
{
    [DDoveEditorPanel("hotbox", "热盒", 160)]
    public sealed class DDoveHotboxEditorPanel : IDDoveEditorPanel
    {
        private readonly DDoveHotboxRingEditor _editor = new DDoveHotboxRingEditor();

        public void Build(VisualElement root)
        {
            root.style.minWidth = 0;
            root.style.minHeight = 0;
            root.style.flexGrow = 1;
            root.style.flexDirection = FlexDirection.Column;

            var hint = new Label("左列拖到右列投放区，按住指令可挪位置。点均匀排布围成一圈。Scene 按 Space 弹出，点中心切换热盒。");
            hint.AddToClassList("ddove-page-desc");
            hint.style.marginTop = 0;
            hint.style.marginBottom = 10;
            hint.style.flexShrink = 0;
            root.Add(hint);

            var host = _editor.Build();
            host.style.flexGrow = 1;
            host.style.minHeight = 0;
            root.Add(host);
        }
    }
}
