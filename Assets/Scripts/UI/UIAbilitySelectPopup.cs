using EEA.AbilitySystem;
using EEA.Define;
using EEA.Manager;
using System.Collections.Generic;
using TableData;
using UnityEngine;

namespace EEA.UI
{
    public class UIAbilitySelectPopup : UIPopupBase
    {
        [SerializeField] private Transform _abilityParent;

        private void OnEnable()
        {
            RandomPeak();
            GameManager.Instance.GameStop();
        }

        private void OnDisable()
        {
            GameManager.Instance.GameResume();
        }

        private void RandomPeak()
        {
            // 아이템과 스킬이 맥스레벨이 아닌 것들을 랜덤으로 3개 선택
            int slotIdx = 0;
            List<AbilityTable> datas = TableManager.Instance.GetDataList<AbilityTable>();
            datas.Sort((a, b) => Random.Range(-1, 2));
            datas.Sort((a, b) => Random.Range(-1, 2));
            datas.Sort((a, b) => Random.Range(-1, 2));

            for (int i = 0; i < datas.Count; ++i)
            {
                SessionAbilityData ability = GameManager.Instance.InventorySessionAbility.GetItem(datas[i].Code);
                if (ability != null && ability.IsMaxLevel() == true)
                    continue;

                UIAbilitySlot slot = ResourceManager.Instance.Create<UIAbilitySlot>(AssetPathUI.UIAbilityItem, _abilityParent);
                slot.Init(datas[i].Code, Hide);
                ++slotIdx;

                if (slotIdx >= 3)
                    break;
            }
        }
    }
}
