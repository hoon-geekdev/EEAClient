using EEA.Manager;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class ALaserBody : Ability
    {
        protected override void OnStart()
        {
            StartCoroutine(Fire());
        }

        private System.Collections.IEnumerator Fire()
        {
            while (true)
            {
                for (int i = 0; i < _count; ++i)
                {
                    Transform unit = PoolManager.Instance.GetObject(_tableData.Asset_path_unit).transform;
                    ALaserUnit projectile = unit.GetComponent<ALaserUnit>();

                    Vector3 pos = new Vector3(
                        _owner.transform.position.x + GetRandomValue(-10f, 10f),
                        _owner.transform.position.y + GetRandomValue(-4f, 4f),
                        0
                    );

                    unit.transform.position = pos;
                    projectile.Init(_damage, _range);

                    yield return new WaitForSeconds(0.1f);
                }

                yield return new WaitForSeconds(_delay);
            }
        }

        private float GetRandomValue(float min, float max)
        {
            System.Random random = new System.Random(System.Guid.NewGuid().GetHashCode());
            return (float)(random.NextDouble() * (max - min) + min);
        }
    }
}
