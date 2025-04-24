using EEA.Define;
using EEA.Manager;
using EEA.Object;
using System.Collections;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class Projectile : MonoBehaviour
    {
        private DamageEvent _damageEvent;
        private float _lifeTime = 4f;
        private Vector3 _moveDirection;
        private float _speed;
        private bool _isPlayerProjectile;
        private string _ownerTag;

        public void Init(DamageEvent evt, float lifetime = 4f)
        {
            _damageEvent = evt;
            _lifeTime = lifetime;
            _speed = evt._speed;
            
            // 발사체 타입 미리 계산하여 저장
            if (evt._owner != null)
            {
                _ownerTag = evt._owner.tag;
                _isPlayerProjectile = _ownerTag == "Player";
            }
            
            // 이동 방향 미리 계산하여 저장
            // direction을 target 방향으로 설정
            if (_damageEvent._target != null)
            {
                _moveDirection = (_damageEvent._target.position - transform.position).normalized;
                // 회전 설정
                transform.up = _moveDirection;
            }
            else
            {
                _moveDirection = transform.up;
            }
            
            StopAllCoroutines();
            StartCoroutine(LifeTime());
        }

        private IEnumerator LifeTime()
        {
            yield return new WaitForSeconds(_lifeTime);
            gameObject.SetActive(false);
        }

        private void Update()
        {
            // 매 프레임마다 _damageEvent 속성에 접근하는 대신 캐시된 값 사용
            transform.Translate(_moveDirection * _speed * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // tag 비교를 위해 매번 _damageEvent._owner에 접근하지 않고 캐시된 값 사용
            if (_isPlayerProjectile)
            {
                if (collision.CompareTag("Enemy"))
                {
                    HandleHit(collision);
                }
            }
            else // 적 발사체
            {
                if (collision.CompareTag("Player"))
                {
                    HandleHit(collision);
                    gameObject.SetActive(false); // 적 발사체는 관통 없이 바로 비활성화
                }
            }
        }

        private void HandleHit(Collider2D collision)
        {
            ObjectBase target = collision.GetComponent<ObjectBase>();
            if (target != null)
            {
                target.TakeDamage(_damageEvent);

                // 관통 처리 (플레이어 발사체만 관통 가능)
                if (_isPlayerProjectile)
                {
                    if (_damageEvent._penetration > 0)
                    {
                        --_damageEvent._penetration;
                        
                        if (_damageEvent._penetration == 0)
                            gameObject.SetActive(false);
                    }
                    else if (_damageEvent._penetration != -1) // -1은 무한 관통
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
