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
                    ProjectileVertical projectile = unit.GetComponent<ProjectileVertical>();
                    //float multiplier = _owner.Status.GetStatus(StatusType.AbilitySpeed, StatusSubType.Multiply);
                    //float speed = _speed + (_speed * multiplier);
                    projectile.Init(_damage, _range, _speed);
                }

                yield return new WaitForSeconds(_delay);
            }
        }
    }
}
