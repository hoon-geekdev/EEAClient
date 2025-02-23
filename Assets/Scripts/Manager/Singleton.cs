using System.Collections;

namespace EEA.Manager
{
    public class Singleton<T> where T : class, new()
    {
        private static T _instance;

        protected Singleton() { }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                        // call Init() method
                        (_instance as Singleton<T>).Init();
                    }
                }

                return _instance;
            }
        }

        public void Init()         
        {
            OnInit();
        }

        virtual public void InitAsync()
        {
        }

        virtual protected void OnInit()
        {
        }
    }
}