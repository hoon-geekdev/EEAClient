using System.Collections;
using UnityEngine;

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
                            if (_instance == null)
                            {
                                GameObject instanceObject = new GameObject();
                                _instance = instanceObject.AddComponent<T>();
                                _instance.name = typeof(T).ToString();
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

        private static T FindInstanceInActiveScene()
        {
            foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
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