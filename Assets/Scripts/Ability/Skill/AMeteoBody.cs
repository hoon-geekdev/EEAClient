using EEA.Manager;
using EEA.Object;
using System.Collections;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class AMeteoBody : Ability
    {
        private Player _player;
        protected override void OnStart()
        {
            _player = _owner as Player;
            StartCoroutine(Fire());
        }

        private IEnumerator Fire()
        {
            while (true)
            {
                for (int i = 0; i < _count; ++i)
                {
                    Transform unit = PoolManager.Instance.GetObject(_tableData.Asset_path_unit).transform;
                    AMeteoUnit projectile = unit.GetComponent<AMeteoUnit>();
                    Vector3 pos = new Vector3(
                        _owner.transform.position.x + GetRandomValue(-10f, 10f),
                        _owner.transform.position.y + GetRandomValue(-4f, 4f),
                        0
                    );

                    unit.transform.position = pos;
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
