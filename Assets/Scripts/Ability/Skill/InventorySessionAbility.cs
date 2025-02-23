using EEA.Manager;
using EEA.Object;
using EEA.SOData;
using System.Collections.Generic;

namespace EEA.AbilitySystem
{
    public class InventorySessionAbility
    {
        Dictionary<long, SessionAbilityItem> _abilities = new Dictionary<long, SessionAbilityItem>();
        public int Count => _abilities.Count;

        public object SOAbilityItem { get; private set; }

        public SessionAbilityItem AddItem(SOSessionAbility itemData, long uid, int level)
        {
            SessionAbilityItem item = new SessionAbilityItem(uid, itemData, level);
            _abilities[uid] = item;

            Player player = GameManager.Instance.Player;
            if (item.Data._type == SOSessionAbility.AbilityType.Item)
            {
                player.Status.AddOrChangeStatus(item.Data._statusType, StatusSubType.Multiply, FromType.SessionAbility, item.UID, item.Ability);
            }

            return item;
        }

        public SessionAbilityItem GetItem(long uid)
        {
            if (_abilities.ContainsKey(uid) == false)
                return null;
            return _abilities[uid];
        }

        public List<SessionAbilityItem> GetItems(bool includeMaxLevel)
        {
            List<SessionAbilityItem> items = new List<SessionAbilityItem>();
            foreach (var item in _abilities.Values)
            {
                if (includeMaxLevel || item.IsMaxLevel == false)
                    items.Add(item);
            }
            return items;
        }
    }
}
