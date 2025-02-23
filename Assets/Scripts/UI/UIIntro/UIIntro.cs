using EEA.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace EEA.UI
{
    public class UIIntro : UIBase
    {
        [SerializeField] private Button _btnStart;
        [SerializeField] private TextMeshProUGUI _txtStart;
        [SerializeField] private TextMeshProUGUI _txtDownload;
        [SerializeField] private Slider _sliderDownload;

        protected override void OnStart()
        {
            base.OnStart();
            SetUITxt();
            SetBtnAction();
        }
        private void SetUITxt()
        {
            SetDownloadText();
        }
        private void SetBtnAction()
        {
            _btnStart.onClick.AddListener(OnClickBtn);
        }

        private void OnClickBtn()
        {
            //GameManager.Instance.TryLogin();
        }

        public void StartBtnEnable(bool isEnable)
        {
            _btnStart.gameObject.SetActive(isEnable);
            _sliderDownload.value = isEnable == false ? 0f : 1f;
            SetDownloadText();

            //if (isEnable)
            //    _txtStart.text = LocalizationUtil.GetName(92010020);
        }

        private void SetDownloadText()
        {
            _txtDownload.text = $"DOWNLOAD {MathF.Floor(_sliderDownload.value * 100f)}%";
        }

        public IEnumerator BundleDownloadStart()
        {
            yield return StartCoroutine(DownloadBundleAsync().AsCoroutine());
        }

        private async Task DownloadBundleAsync()
        {
            List<string> downloadILabels = new List<string>() {
                "Prefab", "UI", "Effect", "Object", "Materials", "Texture", "Animation", "Sound", "Mesh", "Scenes"
            };

            // 각 레이블의 다운로드 크기를 계산
            Dictionary<string, long> downloadSizes = await GetDownloadSizes(downloadILabels);
            long totalDownloadSize = downloadSizes.Values.Sum();

            await DownloadAndTrackProgress(downloadILabels, downloadSizes, totalDownloadSize);
            CompleteDownloadProgress();
        }

        private async Task<Dictionary<string, long>> GetDownloadSizes(List<string> labels)
        {
            Dictionary<string, long> downloadSizes = new Dictionary<string, long>();

            foreach (var label in labels)
            {
                AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(label);
                await getDownloadSize.Task;

                if (getDownloadSize.Status == AsyncOperationStatus.Succeeded)
                {
                    downloadSizes[label] = getDownloadSize.Result;
                }
                else
                {
                    Debug.LogError($"Failed to get download size for label: {label}");
                    downloadSizes[label] = 0;
                }

                Addressables.Release(getDownloadSize);
            }

            return downloadSizes;
        }

        private async Task DownloadAndTrackProgress(List<string> labels, Dictionary<string, long> downloadSizes, long totalDownloadSize)
        {
            float displayedProgressValue = 0f;

            var tasks = labels.Select(async label =>
            {
                if (downloadSizes[label] > 0)
                {
                    Debug.LogWarning($"Download start: {label}");

                    AsyncOperationHandle downloadDependencies = Addressables.DownloadDependenciesAsync(label);

                    while (!downloadDependencies.IsDone)
                    {
                        UpdateDownloadProgress(downloadDependencies, totalDownloadSize, ref displayedProgressValue);

                        await Task.Yield();
                    }

                    Debug.LogWarning($"Download complete: {label}");
                    Addressables.Release(downloadDependencies);
                }
            });

            await Task.WhenAll(tasks);
        }

        private void UpdateDownloadProgress(AsyncOperationHandle downloadDependencies, long totalDownloadSize, ref float displayedProgressValue)
        {
            var downloadStatus = downloadDependencies.GetDownloadStatus();
            long downloadedBytes = downloadStatus.DownloadedBytes;

            float totalProgressValue = (float)downloadedBytes / totalDownloadSize;

            displayedProgressValue = Mathf.Lerp(displayedProgressValue, totalProgressValue, Time.deltaTime * 5);

            if (displayedProgressValue > _sliderDownload.value)
                _sliderDownload.value = displayedProgressValue;
            SetDownloadText();
        }

        private void CompleteDownloadProgress()
        {
            _sliderDownload.value = 1f;
            SetDownloadText();
            Debug.Log("All bundles downloaded!");
        }
    }
}