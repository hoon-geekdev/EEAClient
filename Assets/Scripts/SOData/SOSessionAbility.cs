using EEA.Object;
using UnityEngine;

namespace EEA.SOData
{
    [CreateAssetMenu(fileName = "SOSessionAbility", menuName = "SO/SessionAbility")]
    public class SOSessionAbility : ScriptableObject
    {
        
        public int _id;
        public string _name;
        [TextArea]
        public string _desc;
        public Sprite _icon;

        [Header("Skill Info")]
        public AbilityType _type;
        public StatusType _statusType;
        public GameObject _abilityPref;
        public float _baseSpeed;
        public float _baseDelay;
        public float _baseDuration;

        [Header("Level Info")]
        public float _baseAbility;
        public float[] _abilityByLevel;
        public int[] _countByLevel;
        public int[] _penetrationByLevel;
    }
}
