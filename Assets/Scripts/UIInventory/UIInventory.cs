using EEA.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace EEA.UI
{
    public class UIInventory : UIPopupBase
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private GameObject _item1;
        [SerializeField] private GameObject _item2;

        protected override void OnAwake()
        {
            _closeButton.onClick.AddListener(OnClose);

            if (ItemManager.Instance.GetItems().Count > 0)
            {
                _item1.SetActive(true);
                _item2.SetActive(true);
            }
        }

        private void OnClose()
        {
            UIManager.Instance.DestroyUI(this);
        }
    }
}
