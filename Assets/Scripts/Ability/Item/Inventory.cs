using EEA.Manager;
using EEA.Object;
using System.Collections.Generic;
using EEA.SOData;

namespace EEA.AbilitySystem
{
    public class Inventory
    {
        //Dictionary<long, Item> _items = new Dictionary<long, Item>();
        //public int Count => _items.Count;

        //public void AddItem(SOAbilityItem itemData, long uid, int level)
        //{
        //    Item item = new Item(uid, itemData, level);
        //    _items[uid] = item;

        //    Player player = GameManager.Instance.Player;
        //    if (item.Data._type == SOAbilityItem.ItemType.Shoe)
        //    {
        //        player.Status.AddOrChangeStatus(StatusType.MoveSpeed, StatusSubType.Multiply, FromType.Item, item.UID, item.Ability);
        //    }
        //    else if (item.Data._type == SOAbilityItem.ItemType.Glove)
        //    {
        //        player.Status.AddOrChangeStatus(StatusType.AbilitySpeed, StatusSubType.Multiply, FromType.Item, item.UID, item.Ability);
        //    }
        //}

        //public Item GetItem(long uid)
        //{
        //    if (_items.ContainsKey(uid) == false)
        //        return null;
        //    return _items[uid];
        //}

        //public List<Item> GetItems(bool includeMaxLevel)
        //{
        //    List<Item> items = new List<Item>();
        //    foreach (var item in _items.Values)
        //    {
        //        if (includeMaxLevel || item.IsMaxLevel == false)
        //            items.Add(item);
        //    }
        //    return items;
        //}
    }
}
