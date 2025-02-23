using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EEA.Manager
{
    public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private bool _isDestroyed = false;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = FindInstanceInActiveScene();
                            if (_instance != null)
                            {
                                (_instance as SingletonMono<T>)?.OnAwake();
                            }
                            else
                            {
                                GameObject instanceObject = new GameObject();
                                _instance = instanceObject.AddComponent<T>();
                                (_instance as SingletonMono<T>)?.OnAwake();
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        public bool IsDestroyed => _isDestroyed;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                OnAwake();
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnAwake() { }

        private void OnDestroy()
        {
            _isDestroyed = true;
        }

        private void OnApplicationQuit()
        {
            _isDestroyed = true;
        }

        public virtual IEnumerator InitAsync()
        {
            yield return null;
        }

        public virtual void Init() { }

        private static T FindInstanceInActiveScene()
        {
            foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                T instance = root.GetComponentInChildren<T>(true);
                if (instance != null)
                {
                    return instance;
                }
            }
            return null;
        }
    }
}