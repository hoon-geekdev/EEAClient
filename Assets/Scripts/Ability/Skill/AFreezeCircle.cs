using EEA.Object;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace EEA.AbilitySystem
{
    public class AFreezeCircle : Ability
    {
        private ParticleSystem _particle;

        protected override void OnRefreshData()
        {
            MainModule mainModule = _particle.main;
            mainModule.startSizeMultiplier = _range;
        }

        protected override void OnAwake()
        {
            _particle = GetComponentInChildren<ParticleSystem>();
            _particle.gameObject.SetActive(false);
            StartCoroutine(Effect());
        }

        private IEnumerator Effect()
        {
            while (true)
            {
                yield return new WaitForSeconds(_delay);

                StartCoroutine("HitCheck");
                _particle.gameObject.SetActive(true);
                MainModule main = _particle.main;
                main.simulationSpeed = _particle.main.duration / _duration;
                yield return new WaitForSeconds(_duration);

                _particle.gameObject.SetActive(false);
                StopCoroutine("HitCheck");
            }
        }

        private IEnumerator HitCheck()
        {
            int maxTickCount = Mathf.RoundToInt(_duration / _tick);
            int curCount = 0;
            while (curCount++ < maxTickCount)
            {
                // 2D에서 OverlapCircleAll 사용 (적 레이어만 탐색)
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _range / 2, LayerMask.GetMask("Enemy"));

                foreach (Collider2D collider in colliders)
                {
                    ObjectBase target = collider.GetComponent<ObjectBase>();
                    if (target == null)
                        continue;

                    target.TakeDamage(_damage);
                }

                yield return new WaitForSeconds(_tick);
            }
        }


        private void OnDrawGizmos()
        {
            if (_particle == null)
                return;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _range / 2);
        }
    }
}
