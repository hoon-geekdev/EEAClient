using EEA.AbilitySystem;
using UnityEngine;
using EEA.UI.Controller;
using TMPro;
using UnityEngine.UI;
using EEA.Manager;
using EEA.SOData;
using System;

namespace EEA.UI
{
    public class UIAbilitySlot : MonoBehaviour
    {
        [SerializeField] private ImageEx _imgIcon;
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private TextMeshProUGUI _textDesc;
        [SerializeField] private TextMeshProUGUI _textLevel;
        [SerializeField] private Button _btnLevelUp;

        private long _uid;
        private Action _onCmdClose;

        private void Awake()
        {
            _btnLevelUp.onClick.AddListener(OnClick);
        }

        public void ForceClick()
        {
            OnClick();
        }

        public void Init(long uid, Action onCmdClose)
        {
            _uid = uid;
            string name = string.Empty;

            SessionAbilityItem ability = GameManager.Instance.InventorySessionAbility.GetItem(_uid);
            switch (ability.Data._type)
            {
                case SOSessionAbility.AbilityType.Skill:
                    int value = 0;
                    if (ability.ObjectCount > 0)
                        value = ability.ObjectCount;
                    else if (ability.Penetration > 0)
                        value = ability.Penetration;

                    _textDesc.text = string.Format(ability.Data._desc, ability.Ability * 100, value);
                    _textLevel.text = $"Lv. {ability.Level:D2}";
                    break;
                case SOSessionAbility.AbilityType.Item:
                    _textLevel.text = $"Lv. {ability.Level:D2}";
                    _textDesc.text = string.Format(ability.Data._desc, ability.Ability * 100);
                    break;
                case SOSessionAbility.AbilityType.Heal:
                    _textLevel.text = string.Empty;
                    break;
            }

            _imgIcon.sprite = ability.Data._icon;
            _textName.text = ability.Data._name;

            _onCmdClose = onCmdClose;
        }

        private void OnClick()
        {
            SessionAbilityItem ability = GameManager.Instance.InventorySessionAbility.GetItem(_uid);
            switch (ability.Data._type)
            {
                case SOSessionAbility.AbilityType.Skill:
                    SessionAbilityItem afterItem = GameManager.Instance.InventorySessionAbility.AddItem(ability.Data, _uid, ability.Level + 1);
                    GameManager.Instance.Player.AddOrLevelUpSessionAbility(_uid);
                    break;
                case SOSessionAbility.AbilityType.Item:
                    GameManager.Instance.InventorySessionAbility.AddItem(ability.Data, _uid, ability.Level + 1);
                    break;
                case SOSessionAbility.AbilityType.Heal:
                    GameManager.Instance.Player.Recovery(ability.Data._baseAbility);
                    break;
            }

            _onCmdClose?.Invoke();
        }
    }
}
