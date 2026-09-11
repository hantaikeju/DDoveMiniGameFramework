using UnityEditor;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveUI.Editor
{
    [CustomEditor(typeof(DDoveUIInitInfo))]
    public sealed class DDoveUIInitInfoEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                "运行时 Scaler 与制作场景都读这份 SO。在 DDove/Editor 的 UI 页改。",
                MessageType.Info);
            DrawDefaultInspector();
        }
    }
}
