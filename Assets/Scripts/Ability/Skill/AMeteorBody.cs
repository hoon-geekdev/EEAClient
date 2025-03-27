using EEA.Define;
using EEA.Manager;
using EEA.Object;
using System.Collections;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class AMeteorBody : Ability
    {
        private Player _player;
        private DamageEvent _damageEvent;

        protected override void OnAwake()
        {   
            _player = _owner as Player;
            _damageEvent = new DamageEvent();
        }

        protected override void OnStart()
        {
            StartCoroutine(Fire());
        }

        private IEnumerator Fire()
        {
            while (true)
            {
                for (int i = 0; i < _count; ++i)
                {
                    Transform target = _player.Finder.PeakRandomTarget();
                    if (target == null)
                        break;

                    Transform unit = PoolManager.Instance.GetObject(_tableData.Asset_path_unit).transform;
                    AMeteorUnit projectile = unit.GetComponent<AMeteorUnit>();

                    unit.transform.position = target.position;
                    _damageEvent.Setup(_owner, _damage, _tableData)
                                .SetRange(_range);

                    projectile.Init(_damageEvent);

                    yield return new WaitForSeconds(0.1f);
                }

                yield return new WaitForSeconds(_delay - (_count * 0.1f));
            }
        }

        private float GetRandomValue(float min, float max)
        {
            System.Random random = new System.Random(System.Guid.NewGuid().GetHashCode());
            return (float)(random.NextDouble() * (max - min) + min);
        }
    }
}
