using EEA.Define;
using EEA.Manager;
using EEA.Object;
using System.Collections;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class ASlashSkill : Ability
    {
        private Player _player;
        private DamageEvent _damageEvent;

        protected override void OnAwake()
        {
            _player = _owner as Player;
            _damageEvent = new DamageEvent();
            StartCoroutine(Fire());
        }

        protected override void OnRefreshData()
        {
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
                    float multiplier = _owner.Status.GetStatus(StatusType.Cooltime, StatusSubType.Multiply);

                    // _range만큼 이동 할 때까지의 시간
                    float lifeTime = _range / _speed;
                    _damageEvent.Setup(_owner, _damage, _tableData)
                                .SetSpeed(_speed)
                                .SetPenetration(_penetration);

                    projectile.Init(_damageEvent, lifeTime);
                }

                yield return new WaitForSeconds(_delay);
            }
        }
    }
}
