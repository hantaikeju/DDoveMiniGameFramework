using UnityEditor;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveRes.Editor
{
    [CustomEditor(typeof(DDoveResInitInfo))]
    public sealed class DDoveResInitInfoEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                "InitializeAsync 读这份 SO：默认包（只能有一个）、加载模式、启动场景。\n" +
                "在 DDove/Editor 的 Res 页改；Boot 不拖这份资产。",
                MessageType.Info);
            DrawDefaultInspector();
        }
    }
}