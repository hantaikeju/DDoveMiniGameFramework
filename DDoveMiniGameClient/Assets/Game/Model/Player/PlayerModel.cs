using DDoveFramework.Core;
using UnityEngine;

namespace Game
{
    public struct PlayerItemChangedEvent
    {
        public int ItemId;
        public string Name;
        public int Count;
    }

    [DDoveBindModel]
    public sealed class PlayerModel : AbstractModel
    {
        public const int DemoItemId = 1001;
        private const string CountPrefsKey = "player.demo.item.count";

        public int ItemId { get; private set; } = DemoItemId;

        public string Name { get; private set; } = string.Empty;

        public int Count { get; private set; }

        public bool HasSave { get; private set; }

        protected override void OnInit()
        {
            HasSave = PlayerPrefs.HasKey(CountPrefsKey);
            if (HasSave)
            {
                Count = PlayerPrefs.GetInt(CountPrefsKey, 0);
            }
        }

        public void Apply(int itemId, string name, int count)
        {
            ItemId = itemId;
            Name = name ?? string.Empty;
            Count = count;
            PlayerPrefs.SetInt(CountPrefsKey, Count);
            PlayerPrefs.Save();
            HasSave = true;
            this.SendEvent(new PlayerItemChangedEvent
            {
                ItemId = ItemId,
                Name = Name,
                Count = Count
            });
        }
    }
}
