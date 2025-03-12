using EEA.Define;
using EEA.Manager;
using EEA.UI;
using System.Collections;

namespace EEA.Scene
{
    public class StageScene : SceneStateBase
    {
        protected override IEnumerator OnEnterAsync(params object[] args)
        {
            UIStageHUD hud = UIManager.Instance.CreateUI<UIStageHUD>(AssetPathUI.UIStageHUD);
            GameManager.Instance.InitUserData();
            StageManager.Instance.SetStage(11000001);

            UIJoystick joystick = UIManager.Instance.CreateUI<UIJoystick>(AssetPathUI.UIJoystick);

            yield return base.OnEnterAsync();
        }

        protected override void OnUpdate() { }
        protected override void OnExit() { }
    }
}