
using System; 
namespace TableData {
    [Serializable]
    public class StageWaveTable : BaseTable {
        public string Unique_name { get; set; }
		public int Wave { get; set; }
		public float Time { get; set; }
		public int Monster_id { get; set; }
		public int Spawn_count { get; set; }
		public float Spawn_interval { get; set; }
		public string Spawn_area { get; set; }
		
    }
}