using DG.DemiLib;
using EEA.Define;
using EEA.Object;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class ASlashHitBoxUnit : MonoBehaviour
    {
        private float _angle;
        DamageEvent _damageEvent;

        public void Fire(DamageEvent evt, float lookAngle)
        {
            _damageEvent = evt;
            _angle = lookAngle + (evt._range / 2);

            gameObject.SetActive(true);
        }

        public void Finish()
        {
            gameObject.SetActive(false);
        }

        //private void Update()
        //{
        //    if (_duration > 0)
        //    {
        //        _angle += _range * Time.deltaTime / _duration;
        //        transform.rotation = Quaternion.Euler(0, 0, _angle);
        //    }
        //}

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                collision.GetComponent<Enemy>().TakeDamage(_damageEvent);
            }
        }
    }
}
