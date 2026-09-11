using UnityEditor;
using UnityEngine;

namespace DDoveFramework.Extension.DDoveUI.Editor
{
    internal static class DDoveUIEditorPaths
    {
        public const string InitInfoAssetPath =
            "Assets/DDoveFramework/Extension/DDoveUI/Resources/DDoveUIInitInfo.asset";

        public static DDoveUIInitInfo GetOrCreateInitInfo()
        {
            var info = AssetDatabase.LoadAssetAtPath<DDoveUIInitInfo>(InitInfoAssetPath);
            if (info != null)
            {
                return info;
            }

            const string folder = "Assets/DDoveFramework/Extension/DDoveUI/Resources";
            if (!AssetDatabase.IsValidFolder(folder))
            {
                AssetDatabase.CreateFolder("Assets/DDoveFramework/Extension/DDoveUI", "Resources");
            }

            info = ScriptableObject.CreateInstance<DDoveUIInitInfo>();
            AssetDatabase.CreateAsset(info, InitInfoAssetPath);
            AssetDatabase.SaveAssets();
            return info;
        }

        public static void EnsureFolder(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath) || AssetDatabase.IsValidFolder(assetPath))
            {
                return;
            }

            var parts = assetPath.Replace("\\", "/").Split('/');
            var current = parts[0];
            for (var i = 1; i < parts.Length; i++)
            {
                var next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }

        public const string TemplateDir = "Assets/DDoveFramework/Extension/DDoveUI/Editor/Templates";

        public static string TemplatePath(string fileName)
        {
            return $"{TemplateDir}/{fileName}";
        }
    }
}
