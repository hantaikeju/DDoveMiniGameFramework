using UnityEngine;
using YooAsset;

namespace DDoveFramework.Extension.DDoveRes
{
    [CreateAssetMenu(fileName = "DDoveResInitInfo", menuName = "DDove/Res/Init Info")]
    public sealed class DDoveResInitInfo : ScriptableObject
    {
        public const string ResourcesName = "DDoveResInitInfo";

        [SerializeField] private string _packageName = DDoveResKit.FallbackPackageName;
        [SerializeField] private EPlayMode _playMode = EPlayMode.EditorSimulateMode;
        [SerializeField] private string _launchSceneLocation = DDoveResKit.FallbackLaunchSceneLocation;

        public string PackageName => _packageName;

        public EPlayMode PlayMode => _playMode;

        public string LaunchSceneLocation => _launchSceneLocation;

        public static DDoveResInitInfo Load()
        {
            return Resources.Load<DDoveResInitInfo>(ResourcesName);
        }
    }
}
