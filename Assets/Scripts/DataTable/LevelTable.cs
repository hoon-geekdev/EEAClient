
using System; 
namespace TableData {
    [Serializable]
    public class LevelTable : BaseTable {
        public string Unique_name { get; set; }
		public int Type { get; set; }
		public int Level { get; set; }
		public int Prev_exp { get; set; }
		public int Need_exp { get; set; }
		
    }
}