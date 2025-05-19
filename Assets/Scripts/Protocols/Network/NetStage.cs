using EEA.Manager;
using EEA.Utils;
using Protocols;
using System;
using System.Collections;
using UnityEngine;

namespace EEA.Protocols
{
    public class NetStage
    {
        public void EnterStage(int stageCode, Action<StageEnterRes> onComplete = null) 
        {
            CoroutineHelper.Instance.StartCoroutine(EnterStageCoroutine(stageCode, onComplete));
        }

        public IEnumerator EnterStageCoroutine(int stageCode, Action<StageEnterRes> onComplete = null)
        {
            StageEnterReq req = new StageEnterReq { StageCode = stageCode };
            return HTTPInstance.Instance.SendPostRequestAsync("stage/enter", req, onComplete);
        }

        public void ClearStage(int stageCode, int clearTime, Action<StageClearRes> onComplete = null)
        {
            StageClearReq req = new StageClearReq { StageCode = stageCode, ClearTime = clearTime };
            HTTPInstance.Instance.SendPostRequest("stage/clear", req, onComplete);
        }

        public IEnumerator ClearStageCoroutine(int stageCode, int clearTime, Action<StageClearRes> onComplete = null)
        {
            StageClearReq req = new StageClearReq { StageCode = stageCode, ClearTime = clearTime };
            return HTTPInstance.Instance.SendPostRequestAsync("stage/clear", req, onComplete);
        }
    }
}
