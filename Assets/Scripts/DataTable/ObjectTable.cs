
using System; 
namespace TableData {
    [Serializable]
    public class ObjectTable : BaseTable {
        public string Unique_name { get; set; }
		public int Monster_type { get; set; }
		public int Health { get; set; }
		public float Move_speed { get; set; }
		public float Damage { get; set; }
		public float Range { get; set; }
		public float Attack_cooltime { get; set; }
		public float Attack_speed { get; set; }
		public int[] Drop_items { get; set; }
		public int[] Drop_rates { get; set; }
		public string Asset_path { get; set; }
		public string Ability_asset_path { get; set; }
		
    }
}