using EEA.Manager;
using EEA.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace EEA.Manager
{
    public class UIManager : SingletonMono<UIManager>
    {
        private string _canvasPath = "BuiltIn/UICanvas/UICanvas";
        private UICanvasGroup _canvasGroup;
        private Dictionary<eCanvasType, List<UIBase>> _dicUIList = new Dictionary<eCanvasType, List<UIBase>>();
        private Stack<UIOverlayBase> _backStepUIList = new Stack<UIOverlayBase>();

        protected override void OnAwake()
        {
            GameObject go = Resources.Load<GameObject>(_canvasPath);
            if (go == null)
            {
                Debug.LogError($"UIManager CreateBuildInUI Error : {_canvasPath} is null");
                return;
            }

            _canvasGroup = Instantiate(go, transform).GetComponent<UICanvasGroup>();

            foreach (eCanvasType type in Enum.GetValues(typeof(eCanvasType)))
            {
                _dicUIList[type] = new List<UIBase>();
            }
        }

        public IEnumerator CreateUIAsync<T>(string address, Action<T> callback) where T : UIBase
        {
            eCanvasType type = GetCanvasType<T>();
            Task<T> task = ResourceManager.Instance.CreateAsync<T>(address, _canvasGroup.GetCanvas(type).transform);

            WaitUntil waitForUntil = new WaitUntil(() => task.IsCompleted);
            yield return waitForUntil;

            T ui = task.Result;
            HandleOverlayUI(type, ui);
            callback?.Invoke(ui);
        }

        public void CreateUIAsyncWithStart<T>(string address, Action<T> callback) where T : UIBase
        {
            StartCoroutine(CreateUIAsync<T>(address, callback));
        }

        public T CreateUI<T>(string address) where T : UIBase
        {
            eCanvasType type = GetCanvasType<T>();
            T ui = ResourceManager.Instance.Create<T>(address, _canvasGroup.GetCanvas(type).transform);
            HandleOverlayUI(type, ui);
            return ui;
        }

        public T CreateBuiltInUI<T>(string address) where T : UIBase
        {
            eCanvasType type = GetCanvasType<T>();
            GameObject go = Resources.Load<GameObject>(address);
            if (go == null)
            {
                Debug.LogError($"UIManager CreateBuildInUI Error : {address} is null");
                return null;
            }

            T ui = Instantiate(go).GetComponent<T>();
            ui.transform.SetParent(_canvasGroup.GetCanvas(type).transform, false);
            HandleOverlayUI(GetCanvasType<T>(), ui);
            return ui;
        }
        public Canvas GetCanvas(eCanvasType type)
        {
            if (_canvasGroup == null)
                return null;

            return _canvasGroup.GetCanvas(type);
        }

        public RectTransform GetCanvasResolution(eCanvasType type)
        {
            if (_canvasGroup == null)
                return new RectTransform();

            Canvas canvas = _canvasGroup.GetCanvas(type);
            return canvas.GetComponent<RectTransform>();
        }

        public IEnumerator CreateUIWithParentAsync<T>(string address, Transform parentTransform, Action<T> callback) where T : UIBase
        {
            eCanvasType type = GetCanvasType<T>();
            Task<T> task = ResourceManager.Instance.CreateAsync<T>(address, parentTransform);

            WaitUntil waitForUntil = new WaitUntil(() => task.IsCompleted);
            yield return waitForUntil;

            T ui = task.Result;
            HandleOverlayUI(type, ui);
            callback?.Invoke(ui);
        }

        public T CreateUIWithParent<T>(string address, Transform parentTransform) where T : UIBase
        {
            eCanvasType type = GetCanvasType<T>();
            T ui = ResourceManager.Instance.Create<T>(address, parentTransform);
            HandleOverlayUI(type, ui);
            return ui;
        }

        private void HandleOverlayUI<T>(eCanvasType type, T ui) where T : UIBase
        {
            if (ui is UIOverlayBase overlay)
            {
                CloseCurrentOverlay();
                _backStepUIList.Push(overlay);
            }

            AddUIToDictionary(type, ui);
            ui.Show();
        }

        private void AddUIToDictionary(eCanvasType type, UIBase ui)
        {
            if (_dicUIList.ContainsKey(type))
            {
                _dicUIList[type].Add(ui);
            }
            else
            {
                _dicUIList[type] = new List<UIBase> { ui };
            }
        }

        public void DestroyUI(UIBase ui)
        {
            if (ui == null) return;

            if (ui is UIOverlayBase overlay)
            {
                if (_backStepUIList.Count > 0 && _backStepUIList.Peek() == overlay)
                {
                    _backStepUIList.Pop();
                    ShowNextOverlay();
                }
            }

            eCanvasType type = GetCanvasType(ui.GetType());
            if (_dicUIList.ContainsKey(type) && _dicUIList[type].Contains(ui))
            {
                _dicUIList[type].Remove(ui);
                Destroy(ui.gameObject);
            }
        }

        public void DestroyAll(bool includeLoadingPopup = false)
        {
            List<UIBase> uiToDestroy = new List<UIBase>();

            foreach (var kvp in _dicUIList)
            {
                foreach (var ui in kvp.Value)
                {
                    if (ui is UILoading && !includeLoadingPopup)
                        continue;

                    uiToDestroy.Add(ui);
                }
            }

            foreach (var ui in uiToDestroy)
            {
                DestroyUI(ui);

                foreach (var kvp in _dicUIList)
                {
                    if (kvp.Value.Contains(ui))
                    {
                        kvp.Value.Remove(ui);
                        break;
                    }
                }
            }
            
            //UILoading은 backStepUI가 되면 안됨, 만약 추가할거라면 이 부분 수정 필요
            _backStepUIList.Clear();
        }


        public T GetUI<T>() where T : UIBase
        {
            eCanvasType type = GetCanvasType<T>();
            if (_dicUIList.ContainsKey(type) && _dicUIList[type].Count > 0)
            {
                foreach (var ui in _dicUIList[type])
                {
                    if (ui is T)
                    {
                        return (T)ui;
                    }
                }
            }
            return null;
        }

        private void ShowNextOverlay()
        {
            if (_backStepUIList.Count > 0)
            {
                var nextOverlay = _backStepUIList.Peek();
                nextOverlay.Show();
            }
        }

        private void CloseCurrentOverlay()
        {
            if (_backStepUIList.Count > 0)
            {
                var currentOverlay = _backStepUIList.Peek();
                currentOverlay.Hide();
            }
        }

        private eCanvasType GetCanvasType<T>() where T : UIBase
        {
            return typeof(T) switch
            {
                var uiType when uiType == typeof(UIOverlayBase) => eCanvasType.Overlay,
                var uiType when uiType == typeof(UIPopupBase) => eCanvasType.Popup,
                var uiType when uiType == typeof(UIHudBase) => eCanvasType.Hud,
                _ => eCanvasType.Overlay
            };
        }

        private eCanvasType GetCanvasType(Type uiType)
        {
            return uiType switch
            {
                var t when t == typeof(UIOverlayBase) => eCanvasType.Overlay,
                var t when t == typeof(UIPopupBase) => eCanvasType.Popup,
                var t when t == typeof(UIHudBase) => eCanvasType.Hud,
                _ => eCanvasType.Overlay
            };
        }

        public void RefreshHeaderMatCount()
        {
            bool isCurUIExist = _backStepUIList.TryPeek(out UIOverlayBase curUI);
            if (isCurUIExist == false)
                return;

            //curUI.RefreshMatCount();
        }

        //public void ShowDamage(Transform target, float damage ,bool isCha , bool isCri)
        //{
        //    UIDamageMsg.eDamageType damageType = UIDamageMsg.eDamageType.None;

        //    if (isCha)
        //    {
        //        if (isCri)
        //            damageType = UIDamageMsg.eDamageType.ChaCritical;
        //        else
        //            damageType = UIDamageMsg.eDamageType.ChaNormal;
        //    }
        //    else
        //    {
        //        if (isCri)
        //            damageType = UIDamageMsg.eDamageType.MobCritical;
        //        else
        //            damageType = UIDamageMsg.eDamageType.MobNormal;
        //    }

        //    StartCoroutine(CreateUIAsync<UIDamageMsg>(AssetPathUI.UIDamageMsg, ui =>
        //    {
        //        ui.SetMsg(target,damage, damageType);
        //    }));
        //}        
    }
}

