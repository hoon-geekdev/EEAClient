using EEA.Manager;
using Protocols;
using UnityEngine;
using UnityEngine.UI;

namespace EEA.UI
{
    public class UISystemPopup : UIPopupBase
    {
        [SerializeField] private Button _btnOk;

        protected override void OnAwake()
        {
            _btnOk.onClick.AddListener(OnClickOk);
        }

        private void OnClickOk()
        {
            NetworkManager.Instance.Stage.ClearStage(11000001, (int)GameManager.Instance.GameTime, (StageClearRes res) => {
                SceneManager.Instance.ChangeScene(eScene.LobbyScene);
            });
        }
    }
}
