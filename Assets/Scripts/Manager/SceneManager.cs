using EEA.Define;
using EEA.Scene;
using EEA.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EEA.Manager
{
    public enum eScene
    {
        None,
        IntroScene,
        LobbyScene,
        StageScene,
    }

    public class SceneManager : SingletonMono<SceneManager>
    {
        Dictionary<eScene, SceneStateBase> _sceneDic = new Dictionary<eScene, SceneStateBase>();
        eScene _curScene;

        public eScene CurScene => _curScene;

        protected override void OnAwake()
        {
            _sceneDic.Add(eScene.IntroScene, new IntroScene() { _scenePath = AssetPathScene.Intro });
            _sceneDic.Add(eScene.LobbyScene, new LobbyScene() { _scenePath = AssetPathScene.Lobby });
            _sceneDic.Add(eScene.StageScene, new StageScene() { _scenePath = AssetPathScene.Stage });
        }

        public void ChangeScene(eScene scene, params object[] args)
        {
            StartCoroutine(ChangeSceneCoroutine(scene, args));
        }

        public IEnumerator ChangeSceneAsync(eScene scene, params object[] args)
        {
            yield return StartCoroutine(ChangeSceneCoroutine(scene, args));
        }

        private IEnumerator ChangeSceneCoroutine(eScene scene, params object[] args)
        {
            if (_sceneDic.ContainsKey(scene) == false)
            {
                Debug.Log("Scene not found : " + scene.ToString());
                yield break;
            }

            //CameraManager.Instance.SetCameraBeforeSceneLoad();

            UnityEngine.SceneManagement.Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            if (_sceneDic.ContainsKey(scene))
                _sceneDic[scene].Exit();

            UILoading uILoading = null;
            if (currentScene.name != scene.ToString())
            {
                uILoading = UIManager.Instance.CreateUI<UILoading>(AssetPathUI.UILoading);
                yield return StartCoroutine(_sceneDic[scene].LoadSceneAsync());
            }

            yield return StartCoroutine(_sceneDic[scene].EnterAsync(args));
            //yield return CameraManager.Instance.SetCameraAfterSceneLoadAsync();

            _curScene = scene;
            if (uILoading != null)
                UIManager.Instance.DestroyUI(uILoading);
        }

    }
}