using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DDoveFramework.Editor
{
    public static class DDoveEditorUi
    {
        public const string WindowUssPath = "Assets/DDoveFramework/Editor/DDoveEditorWindow.uss";

        public static void ApplySheet(VisualElement root)
        {
            var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(WindowUssPath);
            if (uss != null && !root.styleSheets.Contains(uss))
            {
                root.styleSheets.Add(uss);
            }
        }

        public static Rect Centered(float width, float height)
        {
            var main = EditorGUIUtility.GetMainWindowPosition();
            return new Rect(
                main.x + (main.width - width) * 0.5f,
                main.y + (main.height - height) * 0.5f,
                width,
                height);
        }

        public static string EditorFolder()
        {
            return Path.GetDirectoryName(WindowUssPath)?.Replace('\\', '/') ?? "Assets/DDoveFramework/Editor";
        }
    }
}
