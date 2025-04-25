using EEA.Manager;
using Newtonsoft.Json;
using Protocols;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace EEA.Manager
{
    public class HTTPInstance : Singleton<HTTPInstance>
    {
        //dev 223.130.158.112
        //k 192.168.0.10
        public string BaseUrl { get; private set; } = "http://localhost:8080/api";
        private Action<ResponsePacketBase> _onRecv;

        public void SetUpdatePacket(Action<ResponsePacketBase> onRecv)
        {
            _onRecv = onRecv;
        }

        public void SendPostRequest<T>(string url, object obj, Action<T> action)
        {
            GameManager.Instance.StartCoroutine(SendWebRequest(url, UnityWebRequest.kHttpVerbPOST, obj, action));
        }

        public IEnumerator SendPostRequestAsync<T>(string url, object obj, Action<T> action)
        {
            yield return GameManager.Instance.StartCoroutine(SendWebRequest(url, UnityWebRequest.kHttpVerbPOST, obj, action));
        }

        private IEnumerator SendWebRequest<T>(string url, string method, object obj, Action<T> onCmdComplete)
        {
            string sendUrl = $"{BaseUrl}/{url}";
            string jsonStr = "";
            byte[] jsonBytes = null;
           
            if (obj != null)
            {
                jsonStr = JsonConvert.SerializeObject(obj);
                jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonStr);
            }

            using (var uwr = new UnityWebRequest(sendUrl, method))
            {
                uwr.uploadHandler = new UploadHandlerRaw(jsonBytes);
                uwr.downloadHandler = new DownloadHandlerBuffer();

                string token = GameManager.Instance.Token;
                if (token != "")
                    uwr.SetRequestHeader("Authorization", "Bearer " + token);

                uwr.SetRequestHeader("Content-Type", "application/json; charset=utf-8");

                Debug.Log($"SendWebRequest: {sendUrl}, Body: {jsonStr}");
                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(uwr.error);
                }
                else
                {
                    T res = JsonConvert.DeserializeObject<T>(uwr.downloadHandler.text);
                    if (_onRecv != null)
                    {
                        _onRecv(res as ResponsePacketBase);
                    }

                    if (onCmdComplete != null)
                    {
                        onCmdComplete(res);
                    }

                    Debug.Log($"RecvWebRequest: {uwr.downloadHandler.text}");
                }
            }
        }
    }
}
