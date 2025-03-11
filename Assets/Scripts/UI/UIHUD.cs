using EEA.Manager;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EEA.UI
{
    public class UIHUD : MonoBehaviour
    {
        public enum InfoType
        {
            Exp, Level, Kill, Time, Health,
        }
        public InfoType _infoType;

        Slider _slider;
        TextMeshProUGUI _text;

        private void Awake()
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _slider = GetComponentInChildren<Slider>();
        }

        private void LateUpdate()
        {
            switch (_infoType)
            {
                case InfoType.Exp:
                    float curExp = GameManager.Instance.GetCurExp();
                    float maxExp = GameManager.Instance.GetNeedExp();
                    float prevLevelExp = GameManager.Instance.GetPrevLevelExp();
                    _slider.value = (curExp - prevLevelExp) / (maxExp - prevLevelExp);
                    break;
                case InfoType.Level:
                    _text.text = $"Lv. {GameManager.Instance.Level + 1}";
                    break;
                case InfoType.Kill:
                    _text.text = $"{GameManager.Instance.KillCount}";
                    break;
                case InfoType.Time:
                    float time = GameManager.Instance.GameTime;
                    TimeSpan timeSpan = TimeSpan.FromSeconds(time);
                    _text.text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                    break;
                case InfoType.Health:
                    float curHp = GameManager.Instance.Player.Health;
                    float maxHp = GameManager.Instance.Player.MaxHealth;

                    _slider.value = curHp / maxHp;
                    break;
            }
        }
    }
}
