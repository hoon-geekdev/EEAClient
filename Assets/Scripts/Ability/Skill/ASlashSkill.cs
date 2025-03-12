using EEA.Manager;
using EEA.Object;
using System.Collections;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class ASlashSkill : Ability
    {
        private Player _player;

        protected override void OnRefreshData()
        {
        }

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
                    unit.position = transform.position;

                    // _player.LookAngle()로 세팅
                    unit.rotation = Quaternion.Euler(0, 0, _player.LookAngle());

                    Projectile projectile = unit.GetComponent<Projectile>();
                    float multiplier = _owner.Status.GetStatus(StatusType.AbilitySpeed, StatusSubType.Multiply);
                    float speed = _speed + (_speed * multiplier);
                    projectile.Init(_damage, speed, _penetration, _duration);
                }

                yield return new WaitForSeconds(_delay);
            }
        }
    }
}
