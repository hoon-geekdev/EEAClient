using System.Collections;
using TMPro;
using UnityEngine;

namespace EEA.UI
{
    public class UILoading : UIBase
    {
        [SerializeField] private TextMeshProUGUI _txtProgress;
        private float _progressValue = 0f;

        protected override void OnStart()
        {
            base.OnStart();
            StartLoading();
        }

        public void SetProgress(float progress)
        {
            StopCoroutine("Progress");
            StartCoroutine(Progress(progress));
        }
        public void StartLoading()
        {
            StopCoroutine("UpdateLoadingText");
            StartCoroutine(UpdateLoadingText());
        }
        private IEnumerator UpdateLoadingText()
        {
            string baseText = "loading";
            int dotCount = 0; 
            while (true)
            {
                _txtProgress.text = baseText + new string('.', dotCount);

                dotCount = (dotCount + 1) % 4;

                yield return new WaitForSeconds(0.2f);
            }
        }
        IEnumerator Progress(float progress)
        {
            while(true)
            {
                // 값을 보간 해서 진행률 표시
                _progressValue = Mathf.Lerp(_progressValue, progress * 100f, Time.deltaTime);
                _txtProgress.text = $"{_progressValue:0}%";
                yield return new WaitForSeconds(0);
            }
        }
    }
}
