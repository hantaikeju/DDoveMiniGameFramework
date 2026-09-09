using UnityEngine;
using YooAsset;

namespace DDoveFramework.Extension.DDoveRes
{
    [CreateAssetMenu(fileName = "DDoveResInitInfo", menuName = "DDove/Res/Init Info")]
    public sealed class DDoveResInitInfo : ScriptableObject
    {
        [SerializeField] private string _packageName = "DefaultPackage";
        [SerializeField] private EPlayMode _playMode = EPlayMode.EditorSimulateMode;

        public string PackageName => _packageName;

        public EPlayMode PlayMode => _playMode;
    }
}
