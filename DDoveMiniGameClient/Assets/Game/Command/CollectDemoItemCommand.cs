using DDoveFramework.Core;

namespace Game
{
    public sealed class CollectDemoItemCommand : AbstractCommand
    {
        public int ItemId = PlayerModel.DemoItemId;

        protected override void OnExecute()
        {
            this.GetSystem<PlayerSystem>().Collect(ItemId);
        }
    }
}
