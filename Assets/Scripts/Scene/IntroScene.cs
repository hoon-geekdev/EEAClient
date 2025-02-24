using EEA.Define;
using EEA.Manager;
using EEA.Scene;
using EEA.UI;
using System.Collections;
using UnityEngine;

namespace EEA.Scene
{
    public class IntroScene : SceneStateBase
    {
        protected override IEnumerator OnEnterAsync(params object[] args)
        {
            UIIntro ui = UIManager.Instance.CreateBuiltInUI<UIIntro>(AssetPathUI.UIIntro);
            ui.StartBtnEnable(false);

            //bool isTableLoaded = false;
            //GameDataManager.Instance.Load(() => { isTableLoaded = true; });

            //WaitUntil waitUntil = new WaitUntil(() => isTableLoaded);
            //if (isTableLoaded == false)
            //    yield return waitUntil;

#if !UNITY_EDITOR || (UNITY_EDITOR && USE_BUNDLE)
            yield return ui.BundleDownloadStart();
#endif
            ui.StartBtnEnable(true);

            // 필요한 초기화 작업을 여기에 추가할 수 있습니다.
            yield return base.OnEnterAsync();
        }

        protected override void OnUpdate() { }
        protected override void OnExit() { }
    }
}