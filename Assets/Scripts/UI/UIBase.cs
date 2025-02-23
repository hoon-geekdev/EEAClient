using UnityEngine;

namespace EEA.UI
{
    public class UIBase : MonoBehaviour
    {
        private void Awake()
        {
            OnAwake();
        }

        private void OnEnable()
        {
            OnActive();
        }

        private void Start()
        {
            OnStart();
        }

        public void Refresh() 
        {
            OnRefresh();
        }

        virtual public void Show()
        {
            gameObject.SetActive(true);
        }

        virtual public void Hide()
        {
            gameObject.SetActive(false);
        }

        virtual protected void OnAwake() { }
        virtual protected void OnStart() { }
        virtual protected void OnActive() { }
        virtual protected void OnRefresh() { }
    }
}
