using EEA.Manager;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace EEA.Scene
{
    public class SceneStateBase
    {
        public string _scenePath;

        public IEnumerator LoadSceneAsync()
        {
            yield return LoadSceneCoroutine(_scenePath);
        }

        public IEnumerator EnterAsync(params object[] args)
        {
            yield return OnEnterAsync(args);
        }

        public void Update()
        {
            OnUpdate();
        }

        public void Exit()
        {
            //ObjectManager.Instance.ClearAllData();
            //PoolManager.Instance.Clear();
            //GameManager.Instance.InitVolume();
            OnExit();
        }

        public virtual IEnumerator LoadSceneFromCodeAsync(params object[] args)
        {
            yield return null;
        }

        protected virtual IEnumerator OnEnterAsync(params object[] args)
        {
            yield return null;
        }

        protected virtual void OnUpdate() { }
        protected virtual void OnExit() { }

        protected IEnumerator LoadSceneCoroutine(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath))
            {
                UnityEngine.Debug.LogError("Scene path is null or empty.");
                yield break;
            }

            UIManager.Instance.DestroyAll();

            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(scenePath, UnityEngine.SceneManagement.LoadSceneMode.Single, true, 1);

            yield return handle;
        }
    }
}