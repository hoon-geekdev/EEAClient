using EEA.Object;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class ASlashHitBoxUnit : MonoBehaviour
    {
        private float _damage = 1f;
        private float _angle;
        private float _range;
        private float _duration;

        public void Fire(float damage, float lookAngle, float duration, float range)
        {
            _damage = damage;
            _range = range;
            _angle = lookAngle + (range / 2);
            _duration = duration;

            gameObject.SetActive(true);
        }

        public void Finish()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_duration > 0)
            {
                _angle += _range * Time.deltaTime / _duration;
                transform.rotation = Quaternion.Euler(0, 0, _angle);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                collision.GetComponent<Enemy>().TakeDamage(_damage);
            }
        }
    }
}
