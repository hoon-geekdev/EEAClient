using EEA.Manager;
using Protocols;
using System;
using System.Collections;

namespace EEA.Protocols
{
    public class NetAdmin
    {
        public IEnumerator AddExp(int addExp, Action<AddExpRes> onComplete = null)
        {
            var req = new AddExpReq { AddExp = addExp };
            return HTTPInstance.Instance.SendPostRequestAsync("admin/account/addExp", req, onComplete);
        }
    }
}
