using EEA.Manager;
using EEA.Object;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class ALaserBody : Ability
    {
        private Player _player;

        protected override void OnStart()
        {
            _player = _owner as Player;
            StartCoroutine(Fire());
        }

        private System.Collections.IEnumerator Fire()
        {
            while (true)
            {
                for (int i = 0; i < _count; ++i)
                {
                    Transform target = _player.Finder.PeakRandomTarget();
                    if (target == null)
                        break;

                    Transform unit = PoolManager.Instance.GetObject(_tableData.Asset_path_unit).transform;
                    ALaserUnit projectile = unit.GetComponent<ALaserUnit>();

                    unit.transform.position = target.position;
                    projectile.Init(_damage, _range);

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
