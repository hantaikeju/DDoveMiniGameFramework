#if UNITY_EDITOR
using UnityEngine;

namespace DDoveFramework.Extension.DDoveUI
{
    public enum DDoveUINodeBindType
    {
        RectTransform = 0,
        Image = 1,
        Text = 2,
        Button = 3,
        TextMeshProUGUI = 4
    }

    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public sealed class DDoveUINodeBind : MonoBehaviour
    {
        public DDoveUINodeBindType ComponentType;
        public string MemberName;

        public string GetMemberName()
        {
            return string.IsNullOrEmpty(MemberName) ? gameObject.name : MemberName;
        }
    }
}
#endif
