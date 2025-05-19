using EEA.Manager;
using Protocols;
using Protocols.DTOs;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EEA.Protocols
{
    public class NetItem
    {
        public IEnumerator AcquireItem(List<ItemAcqDto> items, Action<ItemAcqRes> onComplete = null)
        {
            var req = new ItemAcqReq { Items = items };
            return HTTPInstance.Instance.SendPostRequestAsync("item/acquire", req, onComplete);
        }

        public void GetItemListAsync(Action<ItemListRes> onComplete = null)
        {
            CoroutineHelper.Instance.StartCoroutine(GetItemListCoroutine(onComplete));
        }

        public IEnumerator GetItemListCoroutine(Action<ItemListRes> onComplete = null)
        {
            ItemListReq req = new ItemListReq();
            ItemManager.Instance.ClearItems();

            return HTTPInstance.Instance.SendPostRequestAsync("item/list", req, onComplete);
        }

        public void EquipItem(long characterId, long itemId, Action<ItemEquipRes> onComplete = null)
        {
            var req = new ItemEquipReq { CharacterId = characterId, ItemId = itemId };
            HTTPInstance.Instance.SendPostRequest("item/equip", req, onComplete);
        }

        public void UnequipItem(long itemId, Action<ItemUnequipRes> onComplete = null)
        {
            var req = new ItemUnequipReq { ItemId = itemId };
            HTTPInstance.Instance.SendPostRequest("item/unequip", req, onComplete);
        }

        public IEnumerator LevelupItem(long itemId, Action<ItemLevelupRes> onComplete = null)
        {
            var req = new ItemLevelupReq { ItemId = itemId };
            return HTTPInstance.Instance.SendPostRequestAsync("item/levelup", req, onComplete);
        }

        public IEnumerator EvolveItem(long itemId, long materialItem, Action<ItemEvolveRes> onComplete = null)
        {
            var req = new ItemEvolveReq { ItemId = itemId, MaterialId = materialItem };
            return HTTPInstance.Instance.SendPostRequestAsync("item/evolve", req, onComplete);
        }
    }
}
