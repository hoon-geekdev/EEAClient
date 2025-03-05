using EEA.Manager;
using EEA.Object;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class Ability : MonoBehaviour
    {
        [SerializeField] protected GameObject _unitPref;
        protected ObjectBase _owner;
        protected float _damage;
        protected int _level = 0;
        protected float _speed;
        protected float _delay;
        protected float _duration;
        protected int _penetration;
        protected int _count;
        private long _uid;

        private void Awake()
        {
            _owner = GameManager.Instance.Player;

            OnAwake();
        }

        private void Start()
        {
            OnStart();
        }

        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }

        public void Init(long uid)
        {
            _uid = uid;
            RefreshData();
        }

        public void RefreshData()
        {
            SessionAbilityItem ability = GameManager.Instance.InventorySessionAbility.GetItem(_uid);

            if (ability == null)
            {
                Debug.LogError($"Ability is null. uid: {_uid}");
                return;
            }

            _level = ability.Level;
            _damage = ability.Data._baseAbility * ability.Ability;
            _speed = ability.Data._baseSpeed;
            _delay = ability.Data._baseDelay;
            _count = ability.ObjectCount;
            _penetration = ability.Penetration;
            _duration = ability.Data._baseDuration;

            OnRefreshData();
        }

        protected virtual void OnRefreshData() { }
    }
}
