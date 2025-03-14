using EEA.AbilitySystem;
using EEA.Manager;
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

        public float LookAngle()
        {
            // 캐릭터가 초기에 바라 보는 방향
            if (_prevInputVector == Vector2.zero)
                return 90f;

            float angle = Mathf.Atan2(_prevInputVector.y, _prevInputVector.x) * Mathf.Rad2Deg;
            return angle - 90f;
        }

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

            if (_inputVector != Vector2.zero)
            {
                _animator.SetFloat("SpeedX", _inputVector.x);
                _animator.SetFloat("SpeedY", _inputVector.y);
            }

            _animator.SetBool("IsMove", _inputVector != Vector2.zero);
        }

        public void MoveDir(Vector2 vec)
        {
            _inputVector = vec;
        }

        public void LookDir(Vector2 vec)
        {
            if (vec == Vector2.zero)
                return;

            _prevInputVector = vec;
            _animator.SetFloat("LookX", vec.x);
            _animator.SetFloat("LookY", vec.y);

            if (vec.x != 0)
                _spriteRenderer.flipX = vec.x > 0;
        }

        public void AddOrLevelUpSessionAbility(int tableCode)
        {
            if (_abilities.ContainsKey(tableCode) == false)
            {
                SessionAbilityData abilty = _inventorySessionAbility.GetItem(tableCode);
                if (abilty == null)
                {
                    Debug.LogError($"Not found session ability item. tableCode: {tableCode}");
                    return;
                }

                if (abilty.GetAbilityType() != AbilityType.Skill)
                {
                    Debug.LogError($"Not skill type. tableCode: {tableCode}");
                    return;
                }

                Ability effect = ResourceManager.Instance.Create(abilty.GetAssetPath(), transform).GetComponent<Ability>();
                effect.Init(tableCode);
                _abilities.Add(tableCode, effect);
            }
            else
            {
                Ability effect = _abilities[tableCode];
                effect.RefreshData();
            }
        }

        protected override void OnTakeDamage(float damage)
        {
            base.OnTakeDamage(damage);
            if (IsDead == true)
                return;

            _health -= damage;
        }

        void OnMove(InputValue value)
        {
            _inputVector = value.Get<Vector2>();
        }
    }
}
