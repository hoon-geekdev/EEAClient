using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EEA.Object
{
    public class ObjectBase : MonoBehaviour
    {
        protected Status _status = new Status();
        protected float _health;
        protected Rigidbody2D _rigid;
        protected SpriteRenderer _spriteRenderer;
        protected Collider2D _collider;
        protected Animator _animator;

        public bool IsDead => _health <= 0;
        public float Health => _health;
        public float MaxHealth => _status.GetStatus(StatusType.Health);
        public float MoveSpeed => _status.GetStatus(StatusType.MoveSpeed);
        public Status Status => _status;

        protected void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _collider = GetComponent<Collider2D>();

            OnAwake();
        }

        public void Init(float health, float speed)
        {
            _status.ClearData();
            _status.AddOrChangeStatus(StatusType.Health, StatusSubType.Constant, FromType.Base, (int)DefaultStatusId.Health, health);
            _status.AddOrChangeStatus(StatusType.MoveSpeed, StatusSubType.Constant, FromType.Base, (int)DefaultStatusId.MoveSpeed, speed);

            _health = _status.GetStatus(StatusType.Health);
        }

        public void Recovery(float ratio)
        {
            _health = Mathf.Min(_health + MaxHealth * ratio, MaxHealth);
        }

        protected void Start()
        {
            OnStart();
        }

        protected void Update()
        {
            OnUpdate();
        }

        protected void FixedUpdate()
        {
            OnFixedUpdate();
        }

        protected void LateUpdate()
        {
            OnLateUpdate();
        }

        public void TakeDamage(float damage)
        {
            OnTakeDamage(damage);
        }

        protected virtual void OnTakeDamage(float damage) { }

        protected virtual void OnAwake() { }

        protected virtual void OnStart() { }

        protected virtual void OnUpdate() { }

        protected virtual void OnFixedUpdate() { }

        protected virtual void OnLateUpdate() { }
    }
}
