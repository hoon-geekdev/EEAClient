using EEA.Define;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

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
            if (_damageEvent._penetration == -1 || _damageEvent._penetration > 0)
            {
                if (collision.CompareTag("Enemy"))
                {
                    if (_damageEvent._penetration > 0)
                        --_damageEvent._penetration;

                    collision.GetComponent<Object.ObjectBase>().TakeDamage(_damageEvent);

                    if (_damageEvent._penetration == 0)
                        gameObject.SetActive(false);
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
