using UnityEngine;

namespace DDoveFramework.Extension.DDoveAtlas
{
    [CreateAssetMenu(fileName = "DDoveAtlasInitInfo", menuName = "DDove/Atlas/Init Info")]
    public sealed class DDoveAtlasInitInfo : ScriptableObject
    {
        public const string ResourcesName = "DDoveAtlasInitInfo";
        public const string LogTitle = "DDoveAtlas";
        public const string DefaultSourceRoot = "Assets/GameResExcluded/Atlases";
        public const string DefaultOutputRoot = "Assets/GameRes/Atlases";

        [SerializeField] private string _sourceRoot = DefaultSourceRoot;
        [SerializeField] private string _outputRoot = DefaultOutputRoot;
        [SerializeField] [Range(0, 32)] private int _padding = 4;
        [SerializeField] private FilterMode _filterMode = FilterMode.Bilinear;
        [SerializeField] private bool _pixelArtPreset;

        public string SourceRoot => NonEmpty(_sourceRoot, DefaultSourceRoot);

        public string OutputRoot => NonEmpty(_outputRoot, DefaultOutputRoot);

        public int Padding => _pixelArtPreset ? 8 : _padding;

        public FilterMode FilterMode => _pixelArtPreset ? FilterMode.Point : _filterMode;

        public bool PixelArtPreset => _pixelArtPreset;

        public static DDoveAtlasInitInfo Load()
        {
            return Resources.Load<DDoveAtlasInitInfo>(ResourcesName);
        }

        private static string NonEmpty(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim().Replace('\\', '/');
        }
    }
}
