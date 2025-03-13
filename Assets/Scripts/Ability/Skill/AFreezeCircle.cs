using EEA.Object;
using System.Collections;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class AFreezeCircle : Ability
    {
        private ParticleSystem _particle;

        protected override void OnAwake()
        {
            _particle = GetComponentInChildren<ParticleSystem>();
            _particle.gameObject.SetActive(false);
        }

        protected override void OnRefreshData()
        {
            ParticleSystem.MainModule mainModule = _particle.main;
            mainModule.startSizeMultiplier = _range;

            StopAllCoroutines();
            StartCoroutine(Effect());
        }


        private IEnumerator Effect()
        {
            WaitForSeconds wait1 = new WaitForSeconds(_delay);
            WaitForSeconds wait2 = new WaitForSeconds(_duration);
            while (true)
            {
                yield return wait1;

                StartCoroutine("HitCheck");
                _particle.gameObject.SetActive(true);
                ParticleSystem.MainModule main = _particle.main;
                main.simulationSpeed = _particle.main.duration / _duration;

                yield return wait2;

                _particle.gameObject.SetActive(false);
                StopCoroutine("HitCheck");
            }
        }

        private IEnumerator HitCheck()
        {
            int maxTickCount = Mathf.RoundToInt(_duration / _tick);
            int curCount = 0;
            WaitForSeconds wait = new WaitForSeconds(_tick);
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

                yield return wait;
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
