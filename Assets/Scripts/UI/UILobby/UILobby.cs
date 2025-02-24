using EEA.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace EEA.UI
{
    public class UILobby : UIOverlayBase
    {
        [SerializeField] private Button _btnEnterStage;

        protected override void OnAwake()
        {
            base.OnAwake();

            _btnEnterStage.onClick.AddListener(OnClickEnterStage);
        }

        private void OnClickEnterStage()
        {
            SceneManager.Instance.ChangeScene(eScene.StageScene);
        }
    }
}
