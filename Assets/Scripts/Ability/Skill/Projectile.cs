using EEA.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class Projectile : MonoBehaviour
    {
        private float _damage;
        private float _speed;
        private int _penetration;
        private float _lifeTime = 4f;

        public void Init(float damage, float speed, int penetration)
        {
            _damage = damage;
            _speed = speed;
            _penetration = penetration;

            StartCoroutine(LifeTime());
        }

        private IEnumerator LifeTime()
        {
            yield return new WaitForSeconds(_lifeTime);
            gameObject.SetActive(false);
        }

        private void Update()
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
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

                    if (_penetration <= 0)
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
