using cfg;
using DDoveFramework.Core;
using DDoveFramework.Extension.DDoveCfg;
using Luban.SimpleJSON;

namespace Game
{
    [DDoveBindUtility]
    public sealed class CfgUtility : IUtility
    {
        public Tables Tables { get; }

        public CfgUtility()
        {
            Tables = new Tables(file => JSON.Parse(DDoveCfgKit.RequireText(file)));
        }
    }
}
