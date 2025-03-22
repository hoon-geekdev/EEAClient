using Cinemachine;
using UnityEngine;

namespace EEA.Utils
{
    public class CameraShake : MonoBehaviour
    {
        private CinemachineVirtualCamera _virtualCamera;
        private CinemachineBasicMultiChannelPerlin _perlin;

        public float _shakeDuration = 0.3f;
        public float _shakeAmplitude = 2f;
        public float _shakeFrequency = 2f;

        private float _shakeTimer;
        private bool _isShaking = false;

        void Start()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        public void TriggerShake()
        {
            // 이미 흔들리고 있으면 무시
            if (_isShaking) return;

            _perlin.m_AmplitudeGain = _shakeAmplitude;
            _perlin.m_FrequencyGain = _shakeFrequency;
            _shakeTimer = _shakeDuration;
            _isShaking = true;
        }

        void Update()
        {
            if (_isShaking)
            {
                _shakeTimer -= Time.deltaTime;
                if (_shakeTimer <= 0f)
                {
                    _perlin.m_AmplitudeGain = 0f;
                    _perlin.m_FrequencyGain = 0f;
                    _isShaking = false;
                }
            }
        }
    }
}
