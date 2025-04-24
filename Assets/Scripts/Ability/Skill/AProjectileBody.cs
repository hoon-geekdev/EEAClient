using EEA.Define;
using EEA.Manager;
using EEA.Object;
using System.Collections;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class AProjectileBody : Ability
    {
        private Player _player;
        private DamageEvent _damageEvent;
        
        protected override void OnAwake()
        {
            base.OnAwake();
            _damageEvent = new DamageEvent();
        }
        
        protected override void OnStart()
        {
            _player = _owner as Player;
            StartCoroutine(Fire());
        }

        private IEnumerator Fire()
        {
            WaitForSeconds delayWait = new WaitForSeconds(_delay);
            
            // DamageEvent 기본 설정
            _damageEvent.Setup(_player, _damage, _tableData)
                        .SetSpeed(_speed)
                        .SetPenetration(_penetration);
                                
            while (gameObject.activeSelf)
            {
                for (int i = 0; i < _count; ++i)
                {
                    Transform nearTarget = _player.Finder.PeakNearTarget();
                    if (nearTarget == null)
                        break;

                    Vector3 targetPos = nearTarget.position;
                    Vector3 dir = (targetPos - transform.position).normalized;

                    Transform unit = PoolManager.Instance.GetObject(_tableData.Asset_path_unit).transform;
                    
                    // 오브젝트가 실제로 얻어졌는지 확인
                    if (unit == null) continue;
                    
                    unit.position = transform.position;
                    unit.rotation = Quaternion.FromToRotation(Vector3.up, dir);

                    Projectile projectile = unit.GetComponent<Projectile>();
                    if (projectile != null)
                    {
                        // _damageEvent의 타겟만 업데이트
                        _damageEvent.SetTarget(nearTarget);
                        projectile.Init(_damageEvent);
                    }
                }

                yield return delayWait;
            }
        }
    }
}
