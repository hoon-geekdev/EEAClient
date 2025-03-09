using EEA.Manager;
using EEA.Object;
using EEA.SOData;
using System.Collections.Generic;
using TableData;
using UnityEngine;

namespace EEA.AbilitySystem
{
    
    public class InventorySessionAbility
    {
        Dictionary<long, SessionAbilityData> _abilities = new Dictionary<long, SessionAbilityData>();
        public int Count => _abilities.Count;

        public object SOAbilityItem { get; private set; }

        public SessionAbilityData AddData(int abilityCode, int levelCode)
        {
            AbilityTable abilityTable = TableManager.Instance.GetData<AbilityTable>(abilityCode);
            if (abilityTable == null)
            {
                Debug.LogError($"AbilityTable is null. code: {abilityCode}");
                return null;
            }

            AbilityLevelTable levelTable = TableManager.Instance.GetData<AbilityLevelTable>(levelCode);

            SessionAbilityData data = new SessionAbilityData(abilityTable, levelTable);
            _abilities[abilityTable.Code] = data;

            Player player = GameManager.Instance.Player;
            if (data.GetAbilityType() == AbilityType.Status)
            {
                player.Status.AddOrChangeStatus(data.GetStatusType(), StatusSubType.Multiply, FromType.SessionAbility, data.TableCode, data.GetCalcAbility());
            }

            return data;
        }

        public SessionAbilityData GetItem(long uid)
        {
            if (_abilities.ContainsKey(uid) == false)
                return null;
            return _abilities[uid];
        }

        public List<SessionAbilityData> GetItems(bool includeMaxLevel)
        {
            List<SessionAbilityData> items = new List<SessionAbilityData>();
            foreach (var item in _abilities.Values)
            {
                if (includeMaxLevel || item.IsMaxLevel() == false)
                    items.Add(item);
            }
            return items;
        }
    }
}
