using EEA.SOData;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class SessionAbilityItem
    {
        private long _uid;
        private SOSessionAbility _data;
        private int _level;
        public long UID => _uid;
        public int Level => _level;
        public float Ability => _data._abilityByLevel.Length > _level ? _data._abilityByLevel[_level] : 0f;
        public int ObjectCount => _data._countByLevel.Length > _level ? _data._countByLevel[_level] : 0;
        public int Penetration => _data._penetrationByLevel.Length > _level ? _data._penetrationByLevel[_level] : 0;
        public bool IsMaxLevel => _data._abilityByLevel.Length > 0 && _level >= _data._abilityByLevel.Length - 1;
        public SOSessionAbility Data => _data;
        public SessionAbilityItem(long uid, SOSessionAbility data, int level)
        {
            _uid = uid;
            _data = data;
            _level = level;
        }
    }
}
