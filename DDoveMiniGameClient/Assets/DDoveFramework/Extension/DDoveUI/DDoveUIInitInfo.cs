using UnityEngine;

namespace DDoveFramework.Extension.DDoveUI
{
    [CreateAssetMenu(fileName = "DDoveUIInitInfo", menuName = "DDove/UI/Init Info")]
    public sealed class DDoveUIInitInfo : ScriptableObject
    {
        public const string ResourcesName = "DDoveUIInitInfo";
        public const string LogTitle = "DDoveUI";

        public const string DefaultCreateSceneRoot = "Assets/GameResExcluded/CreateUIScenes";
        public const string DefaultPrefabRoot = "Assets/GameRes/UI";
        public const string DefaultBindScriptsPath = "Assets/Game/Generate/UI";
        public const string DefaultLogicScriptsPath = "Assets/Game/UI";
        public const string DefaultExportRootName = "UIRoot";
        public const string DefaultExcludedBottomName = "Excluded_Bottom";
        public const string DefaultExcludedTopName = "Excluded_Top";
        public const string DefaultNamespace = "Game.UI";

        [SerializeField] private Vector2 _referenceResolution = new Vector2(1080f, 1920f);
        [SerializeField] [Range(0f, 1f)] private float _matchWidthOrHeight = 0.5f;
        [SerializeField] private float _referencePixelsPerUnit = 100f;
        [SerializeField] private int _panelCacheCapacity = 5;
        [SerializeField] private int _uiCameraDepth = 100;
        [SerializeField] private string _createSceneRoot = DefaultCreateSceneRoot;
        [SerializeField] private string _prefabRoot = DefaultPrefabRoot;
        [SerializeField] private string _bindScriptsPath = DefaultBindScriptsPath;
        [SerializeField] private string _logicScriptsPath = DefaultLogicScriptsPath;
        [SerializeField] private string _exportRootName = DefaultExportRootName;
        [SerializeField] private string _excludedBottomName = DefaultExcludedBottomName;
        [SerializeField] private string _excludedTopName = DefaultExcludedTopName;
        [SerializeField] private string _namespaceName = DefaultNamespace;

        public Vector2 ReferenceResolution => _referenceResolution;
        public float MatchWidthOrHeight => _matchWidthOrHeight;
        public float ReferencePixelsPerUnit => _referencePixelsPerUnit;
        public int PanelCacheCapacity => _panelCacheCapacity;
        public int UiCameraDepth => _uiCameraDepth;
        public string CreateSceneRoot => NonEmpty(_createSceneRoot, DefaultCreateSceneRoot);
        public string PrefabRoot => NonEmpty(_prefabRoot, DefaultPrefabRoot);
        public string BindScriptsPath => NonEmpty(_bindScriptsPath, DefaultBindScriptsPath);
        public string LogicScriptsPath => NonEmpty(_logicScriptsPath, DefaultLogicScriptsPath);
        public string ExportRootName => NonEmpty(_exportRootName, DefaultExportRootName);
        public string ExcludedBottomName => NonEmpty(_excludedBottomName, DefaultExcludedBottomName);
        public string ExcludedTopName => NonEmpty(_excludedTopName, DefaultExcludedTopName);
        public string NamespaceName => NonEmpty(_namespaceName, DefaultNamespace);

        public static DDoveUIInitInfo Load()
        {
            return Resources.Load<DDoveUIInitInfo>(ResourcesName);
        }

        public string GetPrefabPath(string stageName, string panelName)
        {
            var stage = string.IsNullOrEmpty(stageName) ? "Start" : stageName;
            return $"{PrefabRoot}/{stage}/{panelName}.prefab";
        }

        public string GetCreateScenePath(string stageName, string panelName)
        {
            var stage = string.IsNullOrEmpty(stageName) ? "Start" : stageName;
            return $"{CreateSceneRoot}/{stage}/{panelName}.unity";
        }

        private static string NonEmpty(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }
    }
}
