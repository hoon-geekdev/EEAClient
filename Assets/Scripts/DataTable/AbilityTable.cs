
using System; 
namespace TableData {
    [Serializable]
    public class AbilityTable : BaseTable {
        public string Unique_name { get; set; }
		public int Type { get; set; }
		public int Status_type { get; set; }
		public float Base_speed { get; set; }
		public float Base_delay { get; set; }
		public float Base_duration { get; set; }
		public float Base_ability { get; set; }
		public float Base_range { get; set; }
		public float Base_tick { get; set; }
		public int[] Level_datas { get; set; }
		public int Name_key { get; set; }
		public int Desc_key { get; set; }
		public string Icon_path { get; set; }
		public string Asset_path { get; set; }
		public string Asset_path_unit { get; set; }
		public string Asset_path_hit { get; set; }
		
    }
}