using Protocols.DTOs;
using System.Collections.Generic;

namespace Protocols
{
    public class StageEnterReq
    {
        public int StageCode { get; set; }
    }

    public class StageEnterRes : ResponsePacketBase
    {
        public bool CanEnter { get; set; }
    }

    public class StageClearReq
    {
        public int StageCode { get; set; }
        public int ClearTime { get; set; }
    }

    public class StageClearRes : ResponsePacketBase
    {

    }
}
