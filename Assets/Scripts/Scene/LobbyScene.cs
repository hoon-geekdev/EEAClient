using EEA.Define;
using EEA.Manager;
using EEA.UI;
using System.Collections;

namespace EEA.Scene
{
    public class LobbyScene : SceneStateBase
    {
        protected override IEnumerator OnEnterAsync(params object[] args)
        {
            UIManager.Instance.CreateUI<UILobby>(AssetPathUI.UILobby);

            yield return base.OnEnterAsync();
        }

        protected override void OnUpdate() { }
        protected override void OnExit() { }
    }
}