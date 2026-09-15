using DDoveFramework.Core;

namespace Game
{
    [DDoveBindSystem]
    public sealed class PlayerSystem : AbstractSystem
    {
        private PlayerModel _player;

        protected override void OnInit()
        {
            _player = this.GetModel<PlayerModel>();
            var item = this.GetUtility<CfgUtility>().Tables.Tbitem.Get(PlayerModel.DemoItemId);
            if (!_player.HasSave)
            {
                _player.Apply(item.Id, item.Name, item.Count);
                return;
            }

            _player.Apply(_player.ItemId, item.Name, _player.Count);
        }

        public void Collect(int itemId)
        {
            var item = this.GetUtility<CfgUtility>().Tables.Tbitem.GetOrDefault(itemId);
            if (item == null)
            {
                DDoveDebug.LogError("Player", ("reason", "item missing"), ("id", itemId));
                return;
            }

            _player.Apply(item.Id, item.Name, _player.Count + 1);
        }
    }
}
