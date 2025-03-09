
using System; 
namespace TableData {
    [Serializable]
    public class ObjectTable : BaseTable {
        public string Unique_name { get; set; }
		public int Health { get; set; }
		public float Move_speed { get; set; }
		public int[] Drop_items { get; set; }
		public int[] Drop_rates { get; set; }
		public string Asset_path { get; set; }
		
    }
}