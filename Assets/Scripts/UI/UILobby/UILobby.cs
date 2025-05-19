using EEA.Define;
using EEA.Manager;
using Protocols;
using UnityEngine;
using UnityEngine.UI;

namespace EEA.UI
{
    public class UILobby : UIOverlayBase
    {
        [SerializeField] private Button _btnEnterStage;
        [SerializeField] private Button _btnBag;

        protected override void OnAwake()
        {
            base.OnAwake();

            _btnEnterStage.onClick.AddListener(OnClickEnterStage);
            _btnBag.onClick.AddListener(OnClickBag);
        }

        private void OnClickEnterStage()
        {
            SceneManager.Instance.ChangeScene(eScene.StageScene);
        }

        private void OnClickBag()
        {
            UIManager.Instance.CreateUI<UIInventory>(AssetPathUI.UIInventory);
        }
    }
}
