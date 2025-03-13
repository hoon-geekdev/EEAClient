using EEA.AbilitySystem;
using UnityEngine;
using EEA.UI.Controller;
using TMPro;
using UnityEngine.UI;
using EEA.Manager;
using EEA.SOData;
using System;
using EEA.Object;
using TableData;
using UnityEditor.Playables;

namespace EEA.UI
{
    public class UIAbilitySlot : MonoBehaviour
    {
        [SerializeField] private ImageEx _imgIcon;
        [SerializeField] private TextMeshProUGUI _textName;
        [SerializeField] private TextMeshProUGUI _textDesc;
        [SerializeField] private TextMeshProUGUI _textLevel;
        [SerializeField] private Button _btnLevelUp;

        private int _tableCode;
        private SessionAbilityData _nextAbilityData;
        private Action _onCmdClose;

        private void Awake()
        {
            _btnLevelUp.onClick.AddListener(OnClick);
        }

        public void ForceClick()
        {
            OnClick();
        }

        public void Init(int tableCode, Action onCmdClose)
        {
            _tableCode = tableCode;
            string name = string.Empty;

            SessionAbilityData curAbility = GameManager.Instance.InventorySessionAbility.GetItem(_tableCode);
            int levelCode;

            AbilityTable abilityTable = TableManager.Instance.GetData<AbilityTable>(_tableCode);
            if (curAbility != null)
                levelCode = curAbility.GetNextLevelCode();
            else
                levelCode = abilityTable.Level_datas.Length > 0 ? abilityTable.Level_datas[0] : 0;

            AbilityLevelTable levelTable = null;
            if (levelCode > 0)
                levelTable = TableManager.Instance.GetData<AbilityLevelTable>(levelCode);

            _nextAbilityData = new SessionAbilityData(abilityTable, levelTable);

            switch (_nextAbilityData.GetAbilityType())
            {
                case AbilityType.Skill:
                    {
                        int curValue = 0;
                        int value = 0;
                        float curAbilityValue = curAbility != null ? curAbility.GetAbility() : 0f;
                        float abilityValue = curAbility == null ? 0 : _nextAbilityData.GetAbility();
                        if (_nextAbilityData.GetCount() > 0)
                        {
                            curValue = curAbility != null ? curAbility.GetCount() : 0;
                            value = _nextAbilityData.GetCount();
                        }
                        else if (_nextAbilityData.GetPenetration() > 0)
                        {
                            curValue = curAbility != null ? curAbility.GetPenetration() : 0;
                            value = _nextAbilityData.GetPenetration();
                        }

                        _textDesc.text = string.Format(_nextAbilityData.GetDesc(), Math.Round((abilityValue - curAbilityValue) * 100, 3), value - curValue);
                        _textLevel.text = $"Lv. {_nextAbilityData.Level:D2}";
                    }
                    
                    break;
                case AbilityType.Status:
                    {
                        float curAbilityValue = curAbility != null ? curAbility.GetAbility() : _nextAbilityData.GetBaseAbility();
                        float abilityValue = _nextAbilityData.GetAbility();
                        _textLevel.text = $"Lv. {_nextAbilityData.Level:D2}";

                        if (abilityValue > 0)
                            _textDesc.text = string.Format(_nextAbilityData.GetDesc(), Math.Round((abilityValue - curAbilityValue) * 100, 3));
                        else
                            _textDesc.text = string.Format(_nextAbilityData.GetDesc(), Math.Round((curAbilityValue) * 100, 3));
                    }
                    break;
                case AbilityType.Consumable:
                    _textLevel.text = string.Empty;
                    break;
            }

            _imgIcon.sprite = ResourceManager.Instance.LoadAsset<Sprite>(_nextAbilityData.GetIconPath());
            _textName.text = _nextAbilityData.GetName();

            _onCmdClose = onCmdClose;
        }

        private void OnClick()
        {
            switch (_nextAbilityData.GetAbilityType())
            {
                case AbilityType.Skill:
                    SessionAbilityData afterItem = GameManager.Instance.InventorySessionAbility.AddData(_tableCode, _nextAbilityData.LevelCode);
                    GameManager.Instance.Player.AddOrLevelUpSessionAbility(_tableCode);
                    break;
                case AbilityType.Status:
                    GameManager.Instance.InventorySessionAbility.AddData(_tableCode, _nextAbilityData.LevelCode);
                    break;
                case AbilityType.Consumable:
                    GameManager.Instance.Player.Recovery(_nextAbilityData.GetCalcAbility());
                    break;
            }

            _onCmdClose?.Invoke();
        }
    }
}
