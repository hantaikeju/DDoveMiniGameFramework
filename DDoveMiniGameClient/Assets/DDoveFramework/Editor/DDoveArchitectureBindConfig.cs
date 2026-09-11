using UnityEditor;
using UnityEngine;

namespace DDoveFramework.Editor
{
    [CreateAssetMenu(fileName = "DDoveArchitectureBindConfig", menuName = "DDove/Architecture/Bind Config", order = 22)]
    public sealed class DDoveArchitectureBindConfig : ScriptableObject
    {
        public const string AssetPath = "Assets/DDoveFramework/Editor/DDoveArchitectureBindConfig.asset";
        public const string DefaultModelPath = "Assets/Game/Model";
        public const string DefaultSystemPath = "Assets/Game/System";
        public const string DefaultUtilityPath = "Assets/Game/Utility";

        [SerializeField] private string _modelPath = DefaultModelPath;
        [SerializeField] private string _systemPath = DefaultSystemPath;
        [SerializeField] private string _utilityPath = DefaultUtilityPath;

        public string ModelPath => Normalize(_modelPath, DefaultModelPath);

        public string SystemPath => Normalize(_systemPath, DefaultSystemPath);

        public string UtilityPath => Normalize(_utilityPath, DefaultUtilityPath);

        public string PathFor(string role)
        {
            switch (role)
            {
                case "Model":
                    return ModelPath;
                case "System":
                    return SystemPath;
                default:
                    return UtilityPath;
            }
        }

        public static DDoveArchitectureBindConfig GetOrCreate()
        {
            var config = AssetDatabase.LoadAssetAtPath<DDoveArchitectureBindConfig>(AssetPath);
            if (config != null)
            {
                return config;
            }

            var folder = System.IO.Path.GetDirectoryName(AssetPath);
            if (!string.IsNullOrEmpty(folder) && !AssetDatabase.IsValidFolder(folder))
            {
                AssetDatabase.CreateFolder("Assets/DDoveFramework", "Editor");
            }

            config = CreateInstance<DDoveArchitectureBindConfig>();
            AssetDatabase.CreateAsset(config, AssetPath);
            AssetDatabase.SaveAssets();
            return config;
        }

        private static string Normalize(string value, string fallback)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallback;
            }

            return value.Trim().Replace("\\", "/");
        }
    }
}
