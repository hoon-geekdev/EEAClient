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
            UIManager.Instance.DestroyUI(this);
        }

        private void RandomPeak()
        {
            // 아이템과 스킬이 맥스레벨이 아닌 것들을 랜덤으로 3개 선택
            int slotIdx = 0;
            List<AbilityTable> datas = TableManager.Instance.GetDataList<AbilityTable>();
            ShuffleList(datas);

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

        private void ShuffleList<T>(List<T> list)
        {
            System.Random rng = new System.Random(); // 시드값을 다르게 주면 매번 다른 순서가 나옴
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1); // 0부터 n까지의 무작위 값
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
