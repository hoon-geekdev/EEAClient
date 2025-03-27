using EEA.Define;
using UnityEngine;

namespace EEA.Object
{
    public class MeleeEnemy : Enemy
    {
        protected override void InitFSM()
        {
            base.InitFSM();
            _states[eEnemyState.Attack] = new MeleeAttackState(this);
        }
    }

    // 특화된 근접 공격 상태
    public class MeleeAttackState : EnemyAttackState
    {
        private MeleeEnemy _meleeEnemy;

        public MeleeAttackState(MeleeEnemy enemy) : base(enemy)
        {
            _meleeEnemy = enemy;
            _attackDuration = 0.5f; // 근접 공격은 빠르게 처리
        }

        public override void Enter()
        {
            base.Enter();
            // 근접 공격 애니메이션 재생 등의 처리
        }

        public override void Execute()
        {
            _attackTimer += Time.deltaTime;
            
            // 근접 공격은 자동으로 처리되므로 시간만 체크
            if (_attackTimer >= _attackDuration)
            {
                _enemy.ChangeState(eEnemyState.Move);
            }
        }
    }
} 