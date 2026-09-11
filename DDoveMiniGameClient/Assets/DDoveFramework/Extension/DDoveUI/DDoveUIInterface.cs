using Cysharp.Threading.Tasks;

namespace DDoveFramework.Extension.DDoveUI
{
    public interface IDDoveUIPanelData
    {
    }

    public interface IDDoveUIPanel
    {
        bool CanOpen();
        UniTask OpenAsync(IDDoveUIPanelData data);
        void Show();
        void Hide();
        void Close();
        bool EnableClose { get; }
    }
}
