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
                "包名 / PlayMode 已改从 Yoo Bundle Collector 读取，Boot 不再引用这份 SO。\n" +
                "编辑器没有收集器 Package 时，运行时用 DefaultPackage 直接调 Yoo。",
                MessageType.Info);
        }
    }
}
