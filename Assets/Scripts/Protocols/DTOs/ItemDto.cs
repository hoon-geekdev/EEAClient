namespace Protocols.DTOs
{
    public class ItemDto
    {
        public long ItemId { get; set; }
        public long? EquippedCharacterId { get; set; }
        public int ItemCode { get; set; }
        public int ItemCount { get; set; }
        public int EvolveCode { get; set; }
        public int LevelCode { get; set; }
    }

    public class ItemAcqDto
    {
        public int ItemCode { get; set; }
        public int Count { get; set; }
    }
}
