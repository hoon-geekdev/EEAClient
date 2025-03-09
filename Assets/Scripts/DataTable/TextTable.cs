
using System; 
namespace TableData {
    [Serializable]
    public class TextTable : BaseTable {
        public string Unique_name { get; set; }
		public string Name_kr { get; set; }
		public string Desc_kr { get; set; }
		
    }
}