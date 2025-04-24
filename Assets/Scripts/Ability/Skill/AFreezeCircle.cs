using EEA.Define;
using EEA.Object;
using System.Collections;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class AFreezeCircle : Ability
    {
        private ParticleSystem _particle;
        private DamageEvent _damageEvent;

        protected override void OnAwake()
        {
            _particle = GetComponentInChildren<ParticleSystem>();
            _particle.gameObject.SetActive(false);
            _damageEvent = new DamageEvent();
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
            
            // 미리 계산된 값들 캐시
            float circleRadius = _range / 2;
            int enemyLayerMask = LayerMask.GetMask("Enemy");
            Vector3 circleCenter = transform.position;
            
            // DamageEvent 기본 설정 - 매 프레임마다 설정하지 않고 한 번만 설정
            _damageEvent.Setup(_owner, _damage, _tableData);
            
            // 최대 타겟 수 제한하여 배열 미리 할당
            const int MAX_TARGETS = 20;
            Collider2D[] hitColliders = new Collider2D[MAX_TARGETS];
            
            while (curCount++ < maxTickCount)
            {
                // OverlapCircleNonAlloc을 사용하여 새 배열 할당을 피함
                int numColliders = Physics2D.OverlapCircleNonAlloc(circleCenter, circleRadius, hitColliders, enemyLayerMask);
                
                for (int i = 0; i < numColliders; i++)
                {
                    ObjectBase target = hitColliders[i].GetComponent<ObjectBase>();
                    if (target == null || !target.gameObject.activeSelf)
                        continue;

                    target.TakeDamage(_damageEvent);
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
