using UnityEngine;

namespace DDoveFramework.Extension.DDoveUI
{
    [DisallowMultipleComponent]
    public sealed class DDoveUIPanelDescription : MonoBehaviour
    {
        public string StageName = "Start";
        public DDoveUIPanelKind PanelKind = DDoveUIPanelKind.Panel;
        public string Namespace = DDoveUIInitInfo.DefaultNamespace;
    }
}
