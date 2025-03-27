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

        public void Init(DamageEvent evt, float lifetime = 4f)
        {
            _damageEvent = evt;
            _lifeTime = lifetime;

            // evt._tartget이 있을 경우 target을 향해 회전
            if (_damageEvent._target != null)
            {
                transform.rotation = Quaternion.FromToRotation(Vector3.up, (_damageEvent._target.position - transform.position).normalized);
            }

            StartCoroutine(LifeTime());
        }

        private IEnumerator LifeTime()
        {
            yield return new WaitForSeconds(_lifeTime);
            gameObject.SetActive(false);
        }

        private void Update()
        {
            transform.Translate(transform.up * _damageEvent._speed * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // _owner가 플레이어인 경우 (플레이어 공격이 적에게 맞음)
            if (_damageEvent._owner != null && _damageEvent._owner.CompareTag("Player"))
            {
                if (collision.CompareTag("Enemy"))
                {
                    HandleHit(collision);
                }
            }
            // _owner가 적인 경우 (적 공격이 플레이어에게 맞음)
            else if (_damageEvent._owner != null && _damageEvent._owner.CompareTag("Enemy"))
            {
                if (collision.CompareTag("Player"))
                {
                    HandleHit(collision);
                    gameObject.SetActive(false); // 적 공격은 관통 없이 바로 비활성화
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
                if (_damageEvent._owner.CompareTag("Player"))
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
