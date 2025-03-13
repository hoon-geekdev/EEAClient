using EEA.Manager;
using EEA.Object;
using System.Collections;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class ProjectileVertical : MonoBehaviour
    {
        private Player _player;
        private ParticleSystem _particle;
        private float _damage;
        private float _speed;
        private float _offset = 4f; // 시작 높이
        private Vector3 _startPos;
        private bool _isFalling = true; // 떨어지는 상태인지 여부
        private float _groundY; // 지면 위치
        private float _range;
        private float _horizontalSpeed; // X축 이동 속도
        private int _direction; // -1 (왼쪽) 또는 1 (오른쪽)

        private void Awake()
        {
            _particle = GetComponentInChildren<ParticleSystem>();
            _player = GameManager.Instance.Player;
        }

        public void Init(float damage, float range, float speed)
        {
            _damage = damage;
            _range = range;
            _speed = speed;

            ParticleSystem.MainModule mainModule = _particle.main;
            mainModule.startSizeMultiplier = range;

            // 랜덤한 위치에서 메테오가 떨어지도록 시작 위치 설정
            _startPos = new Vector3(
                _player.transform.position.x + GetRandomValue(-10, 10),
                _player.transform.position.y + GetRandomValue(-3.5f, 3) + _offset, // 위에서 시작
                0
            );

            transform.position = _startPos;
            _groundY = _startPos.y - _offset; // 지면 위치 설정

            // X축 이동 방향 및 속도 설정 (왼쪽 or 오른쪽)
            _direction = -1;
            _horizontalSpeed = _speed * 0.3f; // Y축 속도의 30% 정도를 X축 이동에 추가 (자연스럽게 낙하)

            _isFalling = true;
            gameObject.SetActive(true);
            StartCoroutine(Fire());
        }

        private float GetRandomValue(float min, float max)
        {
            System.Random random = new System.Random(System.Guid.NewGuid().GetHashCode());
            return (float)(random.NextDouble() * (max - min) + min);
        }

        private IEnumerator Fire()
        {
            while (_isFalling)
            {
                // 메테오가 사선으로 이동 (아래 + 약간 좌/우 이동)
                Vector3 moveDirection = Vector3.down * _speed + Vector3.right * _horizontalSpeed * _direction;
                transform.position += moveDirection * Time.deltaTime;

                // 지면에 닿았을 때
                if (transform.position.y <= _groundY)
                {
                    _isFalling = false;
                    OnImpact();
                }

                yield return null;
            }
        }

        private void OnImpact()
        {
            // 충돌한 적들에게 데미지 적용
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, _range);
            foreach (Collider2D enemy in hitEnemies)
            {
                Enemy enemyHealth = enemy.GetComponent<Enemy>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(_damage);
                }
            }

            // 메테오 제거
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            // 디버깅을 위한 충돌 범위 표시
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _range);
        }
    }
}
