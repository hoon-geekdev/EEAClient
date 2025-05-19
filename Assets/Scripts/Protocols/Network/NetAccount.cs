using EEA.Manager;
using EEA.Utils;
using Protocols;
using System;
using UnityEngine;

namespace EEA.Protocols
{
    public class NetAccount
    {
        public void Login(string account, Action<LoginRes> onComplete)
        {
            var req = new LoginReq { AccountName = account, Password = "123" };
            HTTPInstance.Instance.SendPostRequest("account/login", req, (LoginRes res) =>
            {
                if (res.Token != null)
                {
                    Debug.Log($"Login Success - Token: {res.Token}");
                    AccountInfo.SetToken(res.Token);
                }
                else
                {
                    Debug.Log("Login Failed");
                }

                if (onComplete != null)
                {
                    onComplete(res);
                }
            });
        }

        public void RefreshToken()
        {
            HTTPInstance.Instance.SendPostRequest("account/refreshToken", null, (TokenRefreshRes res) =>
            {
                if (res.Token != null)
                {
                    Debug.Log($"Login Success - Token: {res.Token}");
                    AccountInfo.SetToken(res.Token);
                }
                else
                {
                    Debug.Log("Login Failed");
                }
            });
        }
    }
}
