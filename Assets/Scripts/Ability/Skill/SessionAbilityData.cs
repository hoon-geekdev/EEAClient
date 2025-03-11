using EEA.Manager;
using EEA.Object;
using TableData;
using UnityEditor.Build.Pipeline;

namespace EEA.AbilitySystem
{
    public class SessionAbilityData
    {
        private AbilityTable _data;
        private AbilityLevelTable _levelData;

        public AbilityTable GetData => _data;
        public int TableCode => _data.Code;
        public int Level => _levelData != null ? _levelData.Level : 0;
        public int LevelCode => _levelData != null ? _levelData.Code : 0;

        public SessionAbilityData(AbilityTable table, AbilityLevelTable levelTable)
        {
            _data = table;
            _levelData = levelTable;
        }

        public AbilityType GetAbilityType()
        {
            return (AbilityType)_data.Type;
        }

        public StatusType GetStatusType()
        {
            return (StatusType)_data.Status_type;
        }

        public float GetCalcAbility()
        {
            float multipleValue = _levelData != null ? _levelData.Fixed_value : 1;
            return _data.Base_ability + _data.Base_ability * multipleValue;
        }

        public float GetSpeed()
        {
            return _data.Base_speed;
        }

        public float GetDelay()
        {
            return _data.Base_delay;
        }

        public float GetDuration()
        {
            return _data.Base_duration;
        }

        public float GetCalcRange()
        {
            float multipleValue = _levelData != null ? _levelData.Range : 1;
            return _data.Base_range + _data.Base_range * multipleValue;
        }

        public float GetTick()
        {
            return _data.Base_tick;
        }

        public int GetCount()
        {
            return _levelData == null ? 0 : _levelData.Fixed_count;
        }

        public int GetPenetration()
        {
            return _levelData == null ? 0 : _levelData.Fixed_penetration;
        }

        public float GetAbility()
        {
            return _levelData == null ? _data.Base_ability : _levelData.Fixed_value;
        }

        public float GetBaseAbility()
        {
            return _data.Base_ability;
        }

        public bool IsMaxLevel() 
        {
            return LevelCode >= GetMaxLevel();
        }

        public int GetMaxLevel() 
        { 
            int maxLevel = _data.Level_datas.Length > 0 ? _data.Level_datas[_data.Level_datas.Length - 1] : 0;
            return maxLevel;
        }

        public string GetName()
        {
            TextTable textTable = TableManager.Instance.GetData<TextTable>(_data.Name_key);
            return textTable.Name_kr;
        }

        public string GetDesc()
        {
            TextTable textTable = TableManager.Instance.GetData<TextTable>(_data.Desc_key);
            return textTable.Desc_kr;
        }

        public string GetIconPath()
        {
            return _data.Icon_path;
        }

        public string GetAssetPath()
        {
            return _data.Asset_path;
        }

        public int GetNextLevelCode() 
        { 
            for (int i = 0; i < _data.Level_datas.Length; i++)
            {
                if (_data.Level_datas[i] > LevelCode)
                    return _data.Level_datas[i];
            }

            return 0;
        }
    }
}
