using EEA.AbilitySystem;
using EEA.Manager;
using System.Collections.Generic;
using UnityEngine;

namespace EEA.UI
{
    public class UILevelUpPopup : MonoBehaviour
    {
        [SerializeField] private GameObject _abilitySlotPref;
        [SerializeField] private Transform _abilityParent;

        private List<UIAbilitySlot> _abilities = new List<UIAbilitySlot>();

        private void OnEnable()
        {
            RandomPeak();
            GameManager.Instance.GameStop();
        }

        public void Hide()
        {
            GameManager.Instance.GameResume();
            Destroy(gameObject);
        }

        private void RandomPeak()
        {
            // 아이템과 스킬이 맥스레벨이 아닌 것들을 랜덤으로 3개 선택
            int slotIdx = 0;
            List<SessionAbilityItem> abilities = GameManager.Instance.InventorySessionAbility.GetItems(false);

            while (true)
            {
                if (abilities.Count == 0)
                    break;

                int idx = Random.Range(0, abilities.Count);
                SessionAbilityItem ability = abilities[idx];
                GameObject go = Instantiate(_abilitySlotPref, _abilityParent);
                UIAbilitySlot slot = go.GetComponent<UIAbilitySlot>();
                
                slot.Init(ability.UID, Hide);
                abilities.RemoveAt(idx);

                _abilities.Add(slot);
                slotIdx++;
             
                if (slotIdx >= 3)
                    break;
            }
        }
    }
}
