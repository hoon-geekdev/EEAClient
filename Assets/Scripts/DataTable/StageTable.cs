
using System; 
namespace TableData {
    [Serializable]
    public class StageTable : BaseTable {
        public string Unique_name { get; set; }
		public int[] Wave_1 { get; set; }
		public int[] Wave_2 { get; set; }
		public int[] Reward_items { get; set; }
		
    }
}