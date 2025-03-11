
using System; 
namespace TableData {
    [Serializable]
    public class AbilityLevelTable : BaseTable {
        public string Unique_name { get; set; }
		public int Level { get; set; }
		public float Fixed_value { get; set; }
		public int Fixed_count { get; set; }
		public int Fixed_penetration { get; set; }
		public float Range { get; set; }
		
    }
}