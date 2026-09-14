using System.IO;
using YooAsset.Editor;

namespace DDoveFramework.Extension.DDoveRes.Editor
{
    [DisplayName("收集 SpriteAtlas")]
    public sealed class CollectSpriteAtlas : IAssetFilterRule
    {
        public string FindAssetType => EAssetFilterType.All.ToString();

        public bool IsCollectAsset(AssetFilterRuleData data)
        {
            var ext = Path.GetExtension(data.AssetPath);
            return ext == ".spriteatlas" || ext == ".spriteatlasv2";
        }
    }
}
