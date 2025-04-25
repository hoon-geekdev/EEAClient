using Protocols.DTOs;
using System.Collections.Generic;

namespace Protocols
{
    // ++++++++++++++++++ url: item/acquire ++++++++++++++++++
    // 테스트 목적 이외에는 ItemAcqReq를 사용하지 않음
    public class ItemAcqReq
    {
        public List<ItemAcqDto>? Items { get; set; }
    }

    public class ItemAcqRes: ResponsePacketBase
    {
    }

    // ++++++++++++++++++ url: item/list ++++++++++++++++++
    public class ItemListReq { }
    public class ItemListRes: ResponsePacketBase
    {
    }

    // ++++++++++++++++++ url: item/equip ++++++++++++++++++
    public class ItemEquipReq
    {
        public long CharacterId { get; set; }
        public long ItemId { get; set; }
    }

    public class ItemEquipRes: ResponsePacketBase
    {
    }

    // ++++++++++++++++++ url: item/unequip ++++++++++++++++++
    public class ItemUnequipReq
    {
        public long ItemId { get; set; }
    }

    public class ItemUnequipRes: ResponsePacketBase
    {
    }

    // ++++++++++++++++++ url: item/levelup ++++++++++++++++++
    public class ItemLevelupReq
    {
        public long ItemId { get; set; }
    }

    public class ItemLevelupRes: ResponsePacketBase
    {
    }

    // ++++++++++++++++++ url: item/evolve ++++++++++++++++++
    public class ItemEvolveReq
    {
        public long ItemId { get; set; }
        public long MaterialId { get; set; }
    }

    public class ItemEvolveRes : ResponsePacketBase
    {
    }
}
