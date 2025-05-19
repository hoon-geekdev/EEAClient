using EEA.AbilitySystem;
using EEA.Define;
using EEA.Manager;
using System.Collections;
using UnityEngine;

namespace EEA.Object
{
    public class RangedEnemy : Enemy
    {
        public override void Init(int tableId)
        {
            base.Init(tableId);
            _currentAttackCooldown = 0f;
            _canAttack = true;
        }

        protected override void InitFSM()
        {
            base.InitFSM();
            _states[eEnemyState.Attack] = new RangedAttackState(this);
            _states[eEnemyState.Move] = new RangedMoveState(this);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            
            // 공격 쿨다운 업데이트
            if (!_canAttack)
            {
                _currentAttackCooldown -= Time.deltaTime;
                if (_currentAttackCooldown <= 0f)
                {
                    _canAttack = true;
                }
            }
        }

        // 원거리 공격 구현
        public override void Attack()
        {
            if (!_canAttack || IsDead) return;
            
            _canAttack = false;
            _currentAttackCooldown = _attackCooldown;
            
            FireProjectile();
        }

        // 발사체 생성 및 발사
        private void FireProjectile()
        {
            string projectilePath = _table.Ability_asset_path;
            
            GameObject projectile = PoolManager.Instance.GetObject(projectilePath);
            if (projectile != null)
            {
                projectile.transform.position = transform.position;
                
                Projectile p = projectile.GetComponent<Projectile>();
                if (p != null)
                {
                    // DamageEvent 설정 - 메서드 체이닝 사용
                    _damageEvent.SetTarget(_target.transform)
                                .SetSpeed(_table.Attack_speed);
                    
                    p.Init(_damageEvent);
                }
            }
        }
    }

    // 원거리 몬스터 전용 이동 상태
    public class RangedMoveState : EnemyMoveState
    {
        private RangedEnemy _rangedEnemy;
        private bool _isInAttackRange = false;
        private float _checkRangeInterval = 0.1f;
        private float _lastCheckTime = 0f;

        public RangedMoveState(RangedEnemy enemy) : base(enemy)
        {
            _rangedEnemy = enemy;
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Execute()
        {
            // 불필요한 거리 계산을 줄이기 위해 일정 간격으로만 확인
            if (Time.time - _lastCheckTime >= _checkRangeInterval)
            {
                _lastCheckTime = Time.time;
                float distanceToPlayer = _enemy.GetDistanceToPlayer();
                
                // 공격 범위 안에 있고 공격 가능하면 공격 상태로 전환
                if (distanceToPlayer <= _enemy.AttackRange && _rangedEnemy.CanAttack)
                {
                    _enemy.ChangeState(eEnemyState.Attack);
                }
            }
        }

        public override void FixedExecute()
        {
            if (_rigid == null || _collider == null) return;
            
            float distanceToPlayer = _enemy.GetDistanceToPlayer();
            bool wasInAttackRange = _isInAttackRange;
            _isInAttackRange = distanceToPlayer <= _enemy.AttackRange;
            
            // 공격 범위 상태가 변경될 때만 Rigidbody와 Collider 속성 변경
            if (_isInAttackRange != wasInAttackRange)
            {
                if (_isInAttackRange)
                {
                    // 공격 범위에 들어갔을 때만 Kinematic으로 변경
                    _rigid.bodyType = RigidbodyType2D.Kinematic;
                    _collider.isTrigger = true;
                    _rigid.linearVelocity = Vector2.zero; // linearVelocity 대신 velocity 사용
                }
                else
                {
                    // 공격 범위에서 벗어났을 때만 Dynamic으로 변경
                    _rigid.bodyType = RigidbodyType2D.Dynamic;
                    _collider.isTrigger = false;
                }
            }
            
            // 공격 범위 내에 있으면 이동하지 않음
            // if (_isInAttackRange)  {
            //     return;
            // }

            // 공격 범위 밖이면 플레이어를 향해 이동
            Vector2 dir = _enemy.GetDirectionToPlayer();
            Vector2 moveVec = dir * _enemy.MoveSpeed * Time.fixedDeltaTime;
            _rigid.MovePosition(_rigid.position + moveVec);
            
            // velocity를 0으로 설정하는 것은 필요할 때만 수행
            if (_rigid.linearVelocity.sqrMagnitude > 0.01f)
            {
                _rigid.linearVelocity = Vector2.zero;
            }
        }
    }

    // 원거리 몬스터 전용 공격 상태
    public class RangedAttackState : EnemyAttackState
    {
        private RangedEnemy _rangedEnemy;
        private bool _hasAttacked = false;
        private float _attackDelay = 0.3f; // 공격 애니메이션 시작 후 지연

        public RangedAttackState(RangedEnemy enemy) : base(enemy)
        {
            _rangedEnemy = enemy;
            _attackDuration = 1.5f; // 원거리 공격 상태 지속 시간
        }

        public override void Enter()
        {
            base.Enter();
            _hasAttacked = false;
            _attackTimer = 0f;
        }

        public override void Execute()
        {
            _attackTimer += Time.deltaTime;
            
            // 공격 시작시 한 번만 공격
            if (!_hasAttacked && _attackTimer >= _attackDelay)
            {
                _hasAttacked = true;
                _rangedEnemy.Attack();
            }
            
            // 공격 후 지정된 시간만큼 대기
            if (_attackTimer >= _attackDuration)
            {
                _enemy.ChangeState(eEnemyState.Move);
            }
        }
        
        // FixedExecute 메서드는 비워두어 물리 연산 부하 감소
        public override void FixedExecute() 
        {
            // 공격 중에는 물리 연산 수행하지 않음
        }
    }
} 