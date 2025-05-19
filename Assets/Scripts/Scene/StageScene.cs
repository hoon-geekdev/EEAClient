using EEA.Define;
using EEA.Manager;
using EEA.UI;
using Protocols;
using System.Collections;

namespace EEA.Scene
{
    public class StageScene : SceneStateBase
    {
        protected override IEnumerator OnEnterAsync(params object[] args)
        {
            int enterStageCode = 11000001;
            bool canEnter = false;
            yield return NetworkManager.Instance.Stage.EnterStageCoroutine(enterStageCode, (StageEnterRes res) => {
                canEnter = res.CanEnter;
            });

            UIStageHUD hud = UIManager.Instance.CreateUI<UIStageHUD>(AssetPathUI.UIStageHUD);
            UIJoystick joystick = UIManager.Instance.CreateUI<UIJoystick>(AssetPathUI.UIJoystick);
            GameManager.Instance.InitUserData();
            StageManager.Instance.SetStage(enterStageCode);

            yield return base.OnEnterAsync();
        }

        protected override void OnUpdate() { }
        protected override void OnExit() { }
    }
}