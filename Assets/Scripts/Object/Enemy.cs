using DG.Tweening;
using EEA.Define;
using EEA.Manager;
using EEA.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TableData;
using UnityEngine;

namespace EEA.Object
{
    public enum eMonsterType
    {
        None = 0,
        Normal,
        Elite,
        Boss,
    }

    public enum eEnemyState
    {
        Idle,
        Move,
        Attack,
        Hit,
        Dead
    }

    public class Enemy : ObjectBase
    {
        [SerializeField] protected float _attackRange = 2f;
        protected float _attackCooldown;
        protected float _currentAttackCooldown = 0f;
        protected bool _canAttack = true;
        protected Rigidbody2D _target;
        protected WaitForSeconds _waitForSec = new WaitForSeconds(0);
        protected ObjectTable _table;
        protected bool _isCollidingWithPlayer = false;
        protected DamageEvent _damageEvent;
        protected Collider2D _collider;  // 컴포넌트 캐싱
        protected float _flipTimer = 0f;  // LateUpdate 최적화를 위한 타이머

        protected Material _defaultMaterial;
        protected Material _hitMaterial;
        protected bool _isHiting = false;
        protected Tween _punchTween;
        protected eMonsterType _type;
        protected eEnemyState _currentState;
        protected Dictionary<eEnemyState, IEnemyState> _states;

        protected UIMonsterHealth _uiMonsterHealth;

        private Vector2 _cachedDirectionToPlayer = Vector2.zero;
        private float _cachedDistanceToPlayer = float.MaxValue;
        private float _lastDirectionUpdateTime = 0f;
        private const float DIRECTION_UPDATE_INTERVAL = 0.1f; // 방향 업데이트 간격 (초)

        public eMonsterType MonsterType { get => _type; set => _type = value; }
        public float AttackRange => _attackRange;
        public bool CanAttack => _canAttack;

        public virtual void Init(int tableId)
        {
            _id = tableId;
            _table = TableManager.Instance.GetData<ObjectTable>(_id);
            _type = (eMonsterType)_table.Monster_type;
            
            // DamageEvent 초기화 또는 생성
            if (_damageEvent == null)
                _damageEvent = new DamageEvent();
                
            // DamageEvent 설정
            _damageEvent.Setup(this, _table.Damage);

            _attackCooldown = _table.Attack_cooltime;
            _attackRange = _table.Range;

            _target = GameManager.Instance.Player.GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();  // 캐싱
            _collider.enabled = true;
            _rigid.simulated = true;
            _animator.SetBool("Dead", false);
            _isHiting = false;
            _flipTimer = 0f;

            if (_punchTween != null)
                _punchTween.Kill();
            _punchTween = transform.DOPunchScale(Vector3.one * -0.4f, 0.2f, 10, 1)
            .SetAutoKill(false)
            .Pause();

            if (_type == eMonsterType.Elite)
            {
                _uiMonsterHealth = UIManager.Instance.CreateUI<UIMonsterHealth>(AssetPathUI.UIMonsterHealth);
                _uiMonsterHealth.Init(this);
            }

            InitFSM();
            ChangeState(eEnemyState.Idle);
            base.Init(_table.Health, _table.Move_speed);
        }

        protected virtual void InitFSM()
        {
            _states = new Dictionary<eEnemyState, IEnemyState>();
            _states[eEnemyState.Idle] = new EnemyIdleState(this);
            _states[eEnemyState.Move] = new EnemyMoveState(this);
            _states[eEnemyState.Attack] = new EnemyAttackState(this);
            _states[eEnemyState.Hit] = new EnemyHitState(this);
            _states[eEnemyState.Dead] = new EnemyDeadState(this);
        }

        public void ChangeState(eEnemyState newState)
        {
            if (_currentState == newState) return;

            _states[_currentState]?.Exit();
            _currentState = newState;
            _states[_currentState]?.Enter();
        }

        protected override void OnAwake()
        {
            _defaultMaterial = _spriteRenderer.sharedMaterial;
            _hitMaterial = GameManager.Instance.SharedHitMaterial;
            _states = new Dictionary<eEnemyState, IEnemyState>();
        }

        private void OnEnable()
        {
            StartCoroutine(CheckDistance());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        protected override void OnUpdate()
        {
            if (IsDead) return;
            _states[_currentState]?.Execute();
        }

        protected override void OnFixedUpdate()
        {
            if (IsDead || _states == null || !_states.ContainsKey(_currentState)) return;
            
            // 비활성화된 경우 처리하지 않음
            if (!gameObject.activeSelf) return;
            
            // 유효한 상태만 실행
            _states[_currentState]?.FixedExecute();
        }

        protected override void OnLateUpdate()
        {
            if (IsDead || _target == null) return;
            
            // 매 프레임마다 계산하는 것이 아니라 일정 시간마다 계산하도록 수정
            // 외형에 영향이 적은 최적화이므로 필요하다면 주석 해제
            // _flipTimer += Time.deltaTime;
            // if (_flipTimer >= 0.1f) 
            // {
            //     _flipTimer = 0f;
                _spriteRenderer.flipX = _target.position.x < _rigid.position.x;
            // }
        }

        public Vector2 GetDirectionToPlayer()
        {
            UpdatePlayerPositionCache();
            return _cachedDirectionToPlayer;
        }

        public float GetDistanceToPlayer()
        {
            UpdatePlayerPositionCache();
            return _cachedDistanceToPlayer;
        }

        private void UpdatePlayerPositionCache()
        {
            // 일정 시간마다만 플레이어 방향과 거리를 갱신
            if (Time.time - _lastDirectionUpdateTime < DIRECTION_UPDATE_INTERVAL)
                return;

            _lastDirectionUpdateTime = Time.time;
            
            if (_target == null)
            {
                _cachedDirectionToPlayer = Vector2.zero;
                _cachedDistanceToPlayer = float.MaxValue;
                return;
            }
            
            Vector2 toPlayer = _target.position - _rigid.position;
            _cachedDirectionToPlayer = toPlayer.normalized;
            
            // 제곱근 계산은 필요할 때만 수행 (sqrMagnitude가 더 효율적)
            _cachedDistanceToPlayer = toPlayer.magnitude;
        }

        private IEnumerator CheckDistance()
        {
            // 공유되는 WaitForSeconds 객체를 재사용하여 GC 부하 감소
            WaitForSeconds wait = new WaitForSeconds(0.5f);
            Transform playerTransform = GameManager.Instance.Player.transform;
            
            while (gameObject.activeSelf)
            {
                // Player가 없으면 확인 중단
                if (playerTransform == null) break;
                
                Vector3 dist = playerTransform.position - transform.position;
                
                // 제곱근 계산을 피하기 위해 sqrMagnitude 사용 
                if (dist.sqrMagnitude >= 400f) // 20^2 = 400
                {
                    Vector3 randomPos = new Vector3(
                        Random.Range(10f, 15f), 
                        Random.Range(5f, 10f), 
                        0f
                    );
                    transform.Translate(randomPos + dist.normalized * 10f);
                }

                yield return wait;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 죽었으면 충돌 처리 무시
            if (IsDead) return;
            
            // 플레이어와의 충돌만 처리
            if (collision.CompareTag("Player"))
            {
                // 아직 콜라이딩 상태가 아닐 때만 처리
                if (!_isCollidingWithPlayer)
                {
                    // 플레이어 컴포넌트 캐싱
                    ObjectBase playerObj = collision.GetComponent<ObjectBase>();
                    if (playerObj != null)
                    {
                        // 초기 데미지 적용
                        playerObj.TakeDamage(_damageEvent);
                        
                        // 지속 데미지 코루틴 시작
                        _isCollidingWithPlayer = true;
                        StartCoroutine(DealDamageOverTime(collision));
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _isCollidingWithPlayer = false;
            }
        }

        protected IEnumerator DealDamageOverTime(Collider2D collision)
        {
            // 객체 재사용하여 GC 부하 감소
            WaitForSeconds damageInterval = new WaitForSeconds(2f);
            ObjectBase target = collision.GetComponent<ObjectBase>();
            
            while (_isCollidingWithPlayer && target != null)
            {
                // 객체가 활성화 상태인지 확인
                if (!target.gameObject.activeSelf)
                    break;
                
                // 매번 새 DamageEvent를 만들지 않고 기존 것 재사용
                _damageEvent.SetDamage(_table.Damage);
                target.TakeDamage(_damageEvent);
                
                yield return damageInterval;
            }
            
            _isCollidingWithPlayer = false;
        }

        protected override void OnTakeDamage(DamageEvent evt)
        {
            if (IsDead)
                return;

            _health -= evt._damage;
            
            // 데미지 텍스트 표시 (최적화: 풀에서 객체를 가져오기 전에 확인)
            GameObject damageTextObj = PoolManager.Instance.GetObject(AssetPathUI.UIDamageText);
            if (damageTextObj != null)
            {
                UIDamageText damageText = damageTextObj.GetComponent<UIDamageText>();
                if (damageText != null)
                {
                    damageText.SetText(transform, evt._damage);
                }
            }

            if (IsDead)
            {
                ChangeState(eEnemyState.Dead);
                StartCoroutine(Dead());
            }
            else if (!_isHiting)
            {
                StartCoroutine(Hit(evt));
            }
        }

        protected IEnumerator Hit(DamageEvent evt)
        {
            _isHiting = true;
            ChangeState(eEnemyState.Hit);
            
            // 펀치 트윈 재사용 (새로 생성하지 않음)
            if (_punchTween != null && _punchTween.IsActive())
            {
                _punchTween.Restart();
            }
            
            // 히트 머티리얼 적용
            _spriteRenderer.sharedMaterial = _hitMaterial;

            // 히트 이펙트 표시 (최적화: 필요한 경우에만 생성)
            if (!IsDead)
            {
                string hitEffect = evt._tableData != null ? evt._tableData.Asset_path_hit : AssetPathVFX.DefaultHit;
                GameObject hit = PoolManager.Instance.GetObject(hitEffect);
                if (hit != null)
                {
                    hit.transform.position = transform.position;
                }
            }

            yield return new WaitForSeconds(0.1f);
            
            // 기본 머티리얼 복원
            _spriteRenderer.sharedMaterial = _defaultMaterial;
            _isHiting = false;
            
            if (!IsDead)
                ChangeState(eEnemyState.Move);
        }

        protected IEnumerator KnockBack()
        {
            yield return _waitForSec;
            Vector3 playerPos = GameManager.Instance.Player.transform.position;
            Vector3 dir = (transform.position - playerPos);

            _rigid.AddForce(dir.normalized * 1, ForceMode2D.Impulse);
        }

        protected IEnumerator Dead()
        {
            _collider.enabled = false;
            _rigid.simulated = false;
            _animator.SetBool("Dead", true);

            ItemDrop();
            GameManager.Instance.AddKillCount();
            yield return new WaitForSeconds(0.1f);

            if (_uiMonsterHealth != null)
            {
                UIManager.Instance.DestroyUI(_uiMonsterHealth);
                _uiMonsterHealth = null;
            }
            
            PoolManager.Instance.GetEffect(0).transform.position = transform.position;
            yield return new WaitForSeconds(0.8f);
            gameObject.SetActive(false);
        }

        public virtual void Attack()
        {
            // 자식 클래스에서 구현
        }

        protected void ItemDrop()
        {
            ObjectTable table = TableManager.Instance.GetData<ObjectTable>(_id);
            int sumWeight = table.Drop_rates.Sum();
            int random = Random.Range(0, sumWeight);
            int dropItemCode = 0;

            for (int i = 0; i < table.Drop_items.Length; i++)
            {
                random -= table.Drop_rates[i];
                if (random <= 0)
                {
                    dropItemCode = table.Drop_items[i];
                    break;
                }
            }

            if (dropItemCode != 0)
            {
                ItemTable itemTable = TableManager.Instance.GetData<ItemTable>(dropItemCode);
                GameObject dropGo = PoolManager.Instance.GetObject(itemTable.Asset_path);
                dropGo.transform.position = transform.position;

                DropItem dropItem = dropGo.GetComponent<DropItem>();
                dropItem.Init(dropItemCode);
            }
        }
    }

    public interface IEnemyState
    {
        void Enter();
        void Execute();
        void FixedExecute();
        void Exit();
    }

    #region 기본 상태 클래스
    public class EnemyIdleState : IEnemyState
    {
        protected Enemy _enemy;
        protected float _idleTimer;
        protected float _idleDuration;

        public EnemyIdleState(Enemy enemy)
        {
            _enemy = enemy;
        }

        public virtual void Enter()
        {
            _idleDuration = Random.Range(0.5f, 1.5f);
            _idleTimer = 0f;
        }

        public virtual void Execute()
        {
            _idleTimer += Time.deltaTime;
            if (_idleTimer >= _idleDuration)
            {
                _enemy.ChangeState(eEnemyState.Move);
            }
        }

        public virtual void FixedExecute() { }

        public virtual void Exit() { }
    }

    public class EnemyMoveState : IEnemyState
    {
        protected Enemy _enemy;
        protected Rigidbody2D _rigid;
        protected Collider2D _collider;

        public EnemyMoveState(Enemy enemy)
        {
            _enemy = enemy;
            _rigid = enemy.GetComponent<Rigidbody2D>();
            _collider = enemy.GetComponent<Collider2D>();
        }

        public virtual void Enter() { }

        public virtual void Execute()
        {
            float distanceToPlayer = _enemy.GetDistanceToPlayer();
            
            // 공격 범위 안에 들어오면 공격 상태로 변경
            if (distanceToPlayer <= _enemy.AttackRange)
            {
                _enemy.ChangeState(eEnemyState.Attack);
            }
        }

        public virtual void FixedExecute()
        {
            Vector2 dir = _enemy.GetDirectionToPlayer();
            float distanceToPlayer = _enemy.GetDistanceToPlayer();
            
            // 플레이어와 가까이 붙으면 kinematic으로 변경
            if (distanceToPlayer <= 2f)
            {
                _rigid.bodyType = RigidbodyType2D.Kinematic;
                _collider.isTrigger = true;
            }
            else
            {
                _collider.isTrigger = false;
                _rigid.bodyType = RigidbodyType2D.Dynamic;
            }

            if (distanceToPlayer <= 0.5f)
                return;

            Vector2 moveVec = dir * _enemy.MoveSpeed * Time.fixedDeltaTime;

            _rigid.MovePosition(_rigid.position + moveVec);
            _rigid.linearVelocity = Vector2.zero;
        }

        public virtual void Exit() { }
    }

    public class EnemyAttackState : IEnemyState
    {
        protected Enemy _enemy;
        protected float _attackTimer;
        protected float _attackDuration = 1f;

        public EnemyAttackState(Enemy enemy)
        {
            _enemy = enemy;
        }

        public virtual void Enter()
        {
            _attackTimer = 0f;
        }

        public virtual void Execute()
        {
            _attackTimer += Time.deltaTime;
            
            if (_attackTimer >= _attackDuration)
            {
                _enemy.ChangeState(eEnemyState.Move);
            }
        }

        public virtual void FixedExecute() { }

        public virtual void Exit() { }
    }

    public class EnemyHitState : IEnemyState
    {
        protected Enemy _enemy;

        public EnemyHitState(Enemy enemy)
        {
            _enemy = enemy;
        }

        public virtual void Enter() { }

        public virtual void Execute() { }

        public virtual void FixedExecute() { }

        public virtual void Exit() { }
    }

    public class EnemyDeadState : IEnemyState
    {
        protected Enemy _enemy;

        public EnemyDeadState(Enemy enemy)
        {
            _enemy = enemy;
        }

        public virtual void Enter() { }

        public virtual void Execute() { }

        public virtual void FixedExecute() { }

        public virtual void Exit() { }
    }
    #endregion
}
