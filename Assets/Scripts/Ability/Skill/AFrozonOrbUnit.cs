using EEA.Manager;
using EEA.Object;
using System.Collections;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class AFrozonOrbUnit : Ability
    {
        private float _lifeTime = 4f;
        private Vector3 _dir;

        public void Init(float damage, float speed, int penetration, Transform target)
        {
            _damage = damage;
            _speed = speed;
            _penetration = penetration;
            _dir = (target.position - transform.position).normalized; // 방향을 정규화하여 유지
            StartCoroutine(LifeTime());
            //StartCoroutine(CreateUnit());
        }

        private IEnumerator LifeTime()
        {
            yield return new WaitForSeconds(_lifeTime);
            gameObject.SetActive(false);
        }

        //private IEnumerator CreateUnit()
        //{
        //    for (int i = 0; i < 10; i++)
        //    {
        //        Transform unit = PoolManager.Instance.GetObject(2).transform;
        //        unit.localRotation = Quaternion.identity;
        //        unit.position = transform.position;

        //        Vector3 rot = Vector3.forward * (360 * i / 10);
        //        unit.Rotate(rot);
        //        unit.Translate(unit.up * 1.2f, Space.World);

        //        Projectile projectile = unit.GetComponent<Projectile>();

        //        float multiplier = _owner.Status.GetStatus(StatusType.AbilitySpeed, StatusSubType.Multiply);
        //        float speed = _speed + (_speed * multiplier);
        //        projectile.Init(_damage, speed, _penetration);

        //        yield return new WaitForSeconds(0.1f);
        //    }
        //}

        private void Update()
        {
            // 설정된 방향(_dir)으로 이동
            transform.Translate(_dir * _speed * Time.deltaTime, Space.World);

            // 로컬 축을 기준으로 회전
            transform.Rotate(Vector3.forward * 360 * Time.deltaTime, Space.Self);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                collision.GetComponent<Object.ObjectBase>().TakeDamage(_damage);
            }
        }
    }
}
