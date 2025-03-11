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
        protected float _speed;
        protected float _delay;
        protected float _duration;
        protected int _penetration;
        protected int _count;
        protected float _range;
        protected float _tick;
        private int _tableCode;

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

        public void Init(int tableCode)
        {
            _tableCode = tableCode;
            RefreshData();
        }

        public void RefreshData()
        {
            SessionAbilityData ability = GameManager.Instance.InventorySessionAbility.GetItem(_tableCode);

            if (ability == null)
            {
                Debug.LogError($"Ability is null. uid: {_tableCode}");
                return;
            }

            _damage = ability.GetCalcAbility();
            _speed = ability.GetSpeed();
            _delay = ability.GetDelay();
            _count = ability.GetCount();
            _penetration = ability.GetPenetration();
            _duration = ability.GetDuration();
            _range = ability.GetCalcRange();
            _tick = ability.GetTick();

            OnRefreshData();
        }

        protected virtual void OnRefreshData() { }
    }
}
