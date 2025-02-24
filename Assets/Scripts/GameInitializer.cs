using EEA.Manager;
using System.Collections;
using UnityEngine;

namespace EEA
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private eScene _enterScene = eScene.IntroScene;

        private void Awake()
        {
            Application.runInBackground = true;
            StartCoroutine(Initialization());
        }

        private IEnumerator Initialization()
        {
            eScene curScene = SceneManager.Instance.CurScene;
            if (curScene != eScene.None && curScene == _enterScene)
                yield break;

            //yield return CameraManager.Instance.InitAsync();
            //yield return  UIManager.Instance.InitAsync();

            //SceneManager.Instance.Init();

            yield return SceneManager.Instance.ChangeSceneAsync(_enterScene);
        }
    }
}
