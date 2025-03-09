using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EEA.Object
{
    public enum AbilityType 
    { 
        None,
        Skill, 
        Status,
        Consumable,
    }

    public enum StatusType
    {
        None,
        Health,
        MoveSpeed, 
        AbilitySpeed 
    }

    public enum StatusSubType
    {
        Constant,
        Add,
        Multiply
    }

    public enum FromType
    {
        Base,
        SessionAbility,
        Buff
    }

    public enum DefaultStatusId
    {
        Health = 0,
        MoveSpeed = 1,
    }

    public class Status
    {
        private Dictionary<StatusType, StatusValueGroup> _statusDic = new Dictionary<StatusType, StatusValueGroup>();
        private Dictionary<StatusType, float> _totalValues = new Dictionary<StatusType, float>();

        public void ClearData()
        {
            _statusDic.Clear();
        }

        public void AddOrChangeStatus(StatusType type, StatusSubType subType, FromType fromType, long uid,  float value)
        {
            if (_statusDic.ContainsKey(type) == false)
                _statusDic[type] = new StatusValueGroup();

            _statusDic[type].AddOrChangeStatus(subType, fromType, uid, value);
            CacheStatus(type);
        }

        public float GetStatus(StatusType type)
        {
            if (_totalValues.ContainsKey(type) == false)
                return 0;

            return _totalValues[type];
        }

        public float GetStatus(StatusType type, StatusSubType subType)
        {
            float value = 0;
            if (_statusDic.ContainsKey(type))
            {
                value = _statusDic[type].GetStatus(subType);
            }
            return value;
        }

        private void CacheStatus(StatusType type)
        {
            float value = 0;
            if (_statusDic.ContainsKey(type))
            {
                float constant = _statusDic[type].GetStatus(StatusSubType.Constant);
                float add = _statusDic[type].GetStatus(StatusSubType.Add);
                float multiply = _statusDic[type].GetStatus(StatusSubType.Multiply);

                value = constant + add + (constant * multiply);
            }

            _totalValues[type] = value;
        }
    }

    public class StatusValueGroup
    {
        private Dictionary<StatusSubType, List<StatusValue>> _statusDic = new Dictionary<StatusSubType, List<StatusValue>>();
        private Dictionary<StatusSubType, float> _totalValues = new Dictionary<StatusSubType, float>();
        public void AddOrChangeStatus(StatusSubType subType, FromType fromType, long uid, float value)
        {
            if (_statusDic.ContainsKey(subType) == false)
                _statusDic[subType] = new List<StatusValue>();

            for (int i = 0; i < _statusDic[subType].Count; i++)
            {
                if (_statusDic[subType][i]._fromType == fromType && _statusDic[subType][i]._uid == uid)
                {
                    _statusDic[subType][i]._value = value;
                    CacheStatus(subType);
                    return;
                }
            }

            _statusDic[subType].Add(new StatusValue(value, fromType, uid));
            CacheStatus(subType);
        }

        public float GetStatus(StatusSubType subType)
        {
            if (_totalValues.ContainsKey(subType) == false)
                return 0;

            return _totalValues[subType];
        }

        private void CacheStatus(StatusSubType subType)
        {
            float value = 0;
            if (_statusDic.ContainsKey(subType))
            {
                foreach (var status in _statusDic[subType])
                {
                    value += status._value;
                }
            }

            _totalValues[subType] = value;
        }
    }

    public class StatusValue
    {
        public float _value;
        public FromType _fromType;
        public long _uid;

        public StatusValue(float value, FromType fromType, long uid)
        {
            _value = value;
            _fromType = fromType;
            _uid = uid;
        }
    }
}
