using EEA.AbilitySystem;
using EEA.SOData;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EEA.Object
{
    public class Player : ObjectBase
    {
        private Vector2 _inputVector;
        private Vector2 _prevInputVector;

        private Inventory _inventory;
        private InventorySessionAbility _inventorySessionAbility;
        private Dictionary<long, Ability> _abilities = new Dictionary<long, Ability>();

        public Vector2 InputVec => _inputVector;
        public ObjectFinder Finder { get; private set; }

        public Inventory Inventory => _inventory;
        public InventorySessionAbility InventorySessionAbility => _inventorySessionAbility;

        protected override void OnAwake()
        {
            Finder = GetComponent<ObjectFinder>();
            _inventory = new Inventory();
            _inventorySessionAbility = new InventorySessionAbility();
        }

        protected override void OnFixedUpdate()
        {
            Vector2 moveVector = _inputVector.normalized * MoveSpeed * Time.fixedDeltaTime;
            _rigid.MovePosition(_rigid.position + moveVector);
        }

        protected override void OnLateUpdate()
        {
            bool isMoving = _inputVector != Vector2.zero;
            if (isMoving)
            {
                _animator.SetFloat("SpeedX", _inputVector.x);
                _animator.SetFloat("SpeedY", _inputVector.y);

                _prevInputVector = _inputVector;
                if (_inputVector.x != 0)
                    _spriteRenderer.flipX = _inputVector.x > 0;
            }
            else
            {
                _animator.SetFloat("LookX", _prevInputVector.x);
                _animator.SetFloat("LookY", _prevInputVector.y);
            }

            _animator.SetBool("IsMove", isMoving);
        }

        public void AddOrLevelUpSessionAbility(long uid)
        {
            if (_abilities.ContainsKey(uid) == false)
            {
                SessionAbilityItem abilty = _inventorySessionAbility.GetItem(uid);
                if (abilty == null)
                {
                    Debug.LogError($"Not found session ability item. uid: {uid}");
                    return;
                }

                if (abilty.Data._type != SOSessionAbility.AbilityType.Skill)
                {
                    Debug.LogError($"Not skill type. uid: {uid}");
                    return;
                }

                Ability effect = Instantiate(abilty.Data._abilityPref, transform).GetComponent<Ability>();
                effect.Init(uid);
                _abilities.Add(uid, effect);
            }
            else
            {
                Ability effect = _abilities[uid];
                effect.RefreshData();
            }
        }

        void OnMove(InputValue value)
        {
            _inputVector = value.Get<Vector2>();
        }
    }
}
