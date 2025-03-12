using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace EEA.AbilitySystem
{
    public class Projectile : MonoBehaviour
    {
        private float _damage;
        private float _speed;
        private int _penetration;
        private float _lifeTime = 4f;

        public void Init(float damage, float speed, int penetration, float lifetime = 4f)
        {
            _damage = damage;
            _speed = speed;
            _penetration = penetration;
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
            transform.Translate(transform.up * _speed * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_penetration == -1 || _penetration > 0)
            {
                if (collision.CompareTag("Enemy"))
                {
                    if (_penetration > 0)
                        --_penetration;

                    collision.GetComponent<Object.ObjectBase>().TakeDamage(_damage);

                    if (_penetration == 0)
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
