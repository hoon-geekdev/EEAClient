using Protocols.DTOs;
using System.Collections.Generic;

namespace Protocols
{
    // ++++++++++++++++++ url: admin/account/addExp ++++++++++++++++++
    public class AddExpReq
    {
        public int AddExp { get; set; }
    }

    public class AddExpRes : ResponsePacketBase
    {
    }
    
    // ++++++++++++++++++ url: admin/cheat ++++++++++++++++++
    public class CheatReq
    {
        public string Command { get; set; }
        public List<long> AccountIds { get; set; }
        public List<ItemAcqDto> Items { get; set; }
        public string CheatValue { get; set; }
    }

    public class CheatRes
    {
        public bool success { get; set; }
        public string message { get; set; }
    }
}
