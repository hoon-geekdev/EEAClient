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

        public RangedMoveState(RangedEnemy enemy) : base(enemy)
        {
            _rangedEnemy = enemy;
        }

        public override void Execute()
        {
            float distanceToPlayer = _enemy.GetDistanceToPlayer();
            
            // 공격 범위 안에 있고 공격 가능하면 공격 상태로 전환
            if (distanceToPlayer <= _enemy.AttackRange && _rangedEnemy.CanAttack)
            {
                _enemy.ChangeState(eEnemyState.Attack);
            }
        }

        public override void FixedExecute()
        {
            Vector2 dir = _enemy.GetDirectionToPlayer();
            float distanceToPlayer = _enemy.GetDistanceToPlayer();
            
            // 공격 범위 내에 있으면
            if (distanceToPlayer <= _enemy.AttackRange)
            {
                // Kinematic으로 변경하고 isTrigger 활성화
                _rigid.bodyType = RigidbodyType2D.Kinematic;
                _collider.isTrigger = true;
                
                return;
            }

            // 공격 범위 밖이면 플레이어를 향해 이동
            Vector2 moveVec = dir * _enemy.MoveSpeed * Time.fixedDeltaTime;
            _rigid.MovePosition(_rigid.position + moveVec);
            _rigid.linearVelocity = Vector2.zero;

            // 공격 범위 밖이면 Dynamic으로 변경하고 isTrigger 비활성화
            _rigid.bodyType = RigidbodyType2D.Dynamic;
            _collider.isTrigger = false;
        }
    }

    // 원거리 몬스터 전용 공격 상태
    public class RangedAttackState : EnemyAttackState
    {
        private RangedEnemy _rangedEnemy;
        private bool _hasAttacked = false;

        public RangedAttackState(RangedEnemy enemy) : base(enemy)
        {
            _rangedEnemy = enemy;
            _attackDuration = 1.5f; // 원거리 공격 상태 지속 시간
        }

        public override void Enter()
        {
            base.Enter();
            _hasAttacked = false;
        }

        public override void Execute()
        {
            _attackTimer += Time.deltaTime;
            
            // 공격 시작시 한 번만 공격
            if (!_hasAttacked && _attackTimer >= 0.3f) // 공격 애니메이션 시작 후 약간의 지연
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
    }
} 