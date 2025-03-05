using EEA.Manager;
using EEA.Object;
using System.Collections;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class AFrozonOrb : Ability
    {
        [SerializeField] private int _unitPrefIndex;
        private Player _player;
        private void Start()
        {
            _player = _owner as Player;
            StartCoroutine(Fire());
        }

        private IEnumerator Fire()
        {
            while (true)
            {
                //for (int i = 0; i < _count; ++i)
                //{
                //    Transform nearTarget = _player.Finder.PeakNearTarget();
                //    if (nearTarget == null)
                //        break;

                //    Vector3 targetPos = nearTarget.position;
                //    Vector3 dir = (targetPos - transform.position).normalized;

                //    Transform unit = PoolManager.Instance.GetObject(_unitPrefIndex).transform;
                //    unit.position = transform.position;
                //    unit.rotation = Quaternion.FromToRotation(Vector3.up, dir);

                //    Projectile projectile = unit.GetComponent<Projectile>();
                //    float multiplier = _owner.Status.GetStatus(StatusType.AbilitySpeed, StatusSubType.Multiply);
                //    float speed = _speed + (_speed * multiplier);
                //    projectile.Init(_damage, speed, _penetration);
                //}

                Transform nearTarget = _player.Finder.PeakNearTarget();
                if (nearTarget != null)
                {
                    Vector3 targetPos = nearTarget.position;
                    Vector3 dir = (targetPos - transform.position).normalized;

                    Transform unit = PoolManager.Instance.GetObject(_unitPrefIndex).transform;
                    unit.position = transform.position;
                    unit.rotation = Quaternion.FromToRotation(Vector3.up, dir);

                    AFrozonOrbUnit projectile = unit.GetComponent<AFrozonOrbUnit>();
                    float multiplier = _owner.Status.GetStatus(StatusType.AbilitySpeed, StatusSubType.Multiply);
                    float speed = _speed + (_speed * multiplier);
                    projectile.Init(_damage, speed, _penetration, nearTarget);
                }

                yield return new WaitForSeconds(_delay);
            }
        }
    }
}
