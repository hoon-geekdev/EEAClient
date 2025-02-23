using EEA.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace EEA.Manager
{
    public class ResourceManager : SingletonMono<ResourceManager>
    {
        private readonly HashSet<CancellationTokenSource> _tokens = new();
        private readonly object _tokensLock = new object();

        public async Task<T> LoadAssetAsync<T>(string address, CancellationToken externalToken = default) where T : UnityEngine.Object
        {
            if (IsDestroyed == true)
                return null;
                
            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            // 토큰 관리에 추가
            lock (_tokensLock)
            {
                _tokens.Add(linkedCts);
            }

            try
            {
                AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);

                // 로드 작업 대기 (토큰으로 취소 가능)
                T result = await handle.Task.WithCancellation(linkedCts.Token);

                if (handle.Status == AsyncOperationStatus.Succeeded && linkedCts.Token.IsCancellationRequested == false)
                {
                    return result;
                }
                else
                {
                    Debug.LogError($"Failed to load asset at address: {address}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception occurred while loading asset at address: {address}. Exception: {ex.Message}");
                return null;
            }
            finally
            {
                // 토큰 관리에서 제거
                lock (_tokensLock)
                {
                    _tokens.Remove(linkedCts);
                }
            }
        }

        public void CancelAll()
        {
            lock (_tokensLock)
            {
                for (int i = _tokens.Count - 1; i >= 0; i--)
                {
                    CancellationTokenSource cts = _tokens.ElementAt(i);
                    if (cts.Token.CanBeCanceled)
                    {
                        try
                        {
                            cts.Cancel();
                        }
                        catch (OperationCanceledException)
                        {
                            // 이미 취소된 경우 무시
                        }
                    }
                }
                _tokens.Clear();
            }
        }

        private void OnApplicationQuit()
        {
            CancelAll();
        }

        public async Task<T> CreateAsync<T>(string address, Transform parent) where T : UnityEngine.Object
        {
            GameObject obj = await LoadAssetAsync<GameObject>(address);
            if (obj == null)
                return null;

            GameObject instance = Instantiate(obj, parent);

            T component = instance.GetComponent<T>();
            return component;
        }

        public async Task<T> CreateAsync<T>(string address, bool dontDestroyOnLoad = true) where T : UnityEngine.Object
        {
            GameObject instance = await CreateAsync(address);
            T component = instance.GetComponent<T>();

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(instance);
            }
            return component;
        }

        public async Task<GameObject> CreateAsync(string address)
        {
            GameObject obj = await LoadAssetAsync<GameObject>(address);
            if (obj == null)
                return null;

            GameObject instance = Instantiate(obj);
            return instance;
        }

        public T Create<T>(T obj, Vector3 position, Quaternion rotation) where T : UnityEngine.Object
        {
            T instance = Instantiate(obj, position, rotation);
            return instance;
        }

        public T Create<T>(string address, Vector3 position, Quaternion rotation) where T : UnityEngine.Object
        {
            GameObject obj = Create(address, position, rotation);
            T component = obj.GetComponent<T>();

            return component;
        }

        public T Create<T>(string address, Vector3 position, Quaternion rotation, Transform parent) where T : UnityEngine.Object
        {
            GameObject obj = Create(address, position, rotation, parent);
            T component = obj.GetComponent<T>();

            return component;
        }

        public GameObject Create(string address, Vector3 position, Quaternion rotation)
        {
            GameObject obj = LoadAsset<GameObject>(address);
            GameObject instance = Instantiate(obj, position, rotation);

            return instance;
        }

        public GameObject Create(string address, Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject obj = LoadAsset<GameObject>(address);
            GameObject instance = Instantiate(obj, position, rotation, parent);

            return instance;
        }

        public T Create<T>(string address, Transform parent = null) where T : UnityEngine.Object
        {
            GameObject go = Create(address, parent);
            T instance = go.GetComponent<T>();

            return instance;
        }

        public GameObject Create(string address, Transform parent = null)
        {
            GameObject obj = LoadAsset<GameObject>(address);
            GameObject instance = Instantiate(obj, parent);

            return instance;
        }

        // Addressable Asset을 언로드하는 메서드
        public void UnloadAsset<T>(T asset) where T : UnityEngine.Object
        {
            Addressables.Release(asset);
        }

        public void DestroyMyObject(GameObject obj)
        {
            if (obj == null)
                return;

            Destroy(obj);
        }

        public T LoadAsset<T>(string address)
        {
            try
            {
                T obj = Addressables.LoadAssetAsync<T>(address).WaitForCompletion();
                return obj;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception occurred while loading asset at address: {address}. Exception: {ex.Message}");
                return default;
            }
        }
    }
}