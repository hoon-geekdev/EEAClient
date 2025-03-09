
using System; 
namespace TableData {
    [Serializable]
    public class ItemTable : BaseTable {
        public string Unique_name { get; set; }
		public int Type { get; set; }
		public float Ability { get; set; }
		public string Drop_icon_path { get; set; }
		public string Asset_path { get; set; }
		
    }
}