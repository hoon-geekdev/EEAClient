using System.Collections.Generic;
using Protocols.DTOs;
using UnityEngine;

namespace EEA.Manager
{
    public class ItemManager : Singleton<ItemManager>
    {
        private Dictionary<long, ItemDto> _items = new Dictionary<long, ItemDto>();
        public void ClearItems()
        {
            // 아이템 목록 초기화
            _items.Clear();
        }

        public void AddItem(ItemDto item)
        {
            _items[item.ItemId] = item;
        }

        public void RemoveItem(ItemDto item)
        {
            _items.Remove(item.ItemId);
        }

        public ItemDto GetItem(long itemId)
        {
            return _items.TryGetValue(itemId, out var item) ? item : null;
        }

        public List<ItemDto> GetItems()
        {
            return new List<ItemDto>(_items.Values);
        }
    }
}
