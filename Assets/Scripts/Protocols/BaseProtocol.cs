using Protocols.DTOs;
using System.Collections.Generic;

namespace Protocols
{
    public class ResponsePacketBase
    {
        public string Error { get; set; } = "";
        public UpdatePacket UpdateData { get; set; } = new UpdatePacket();
    }

    public class UpdatePacket
    {
        public AccountDto? Account { get; set; }
        public List<ItemDto>? Items { get; set; }
    }
}
