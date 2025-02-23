using EEA.Object;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class ASpinShovel : Ability
    {
        protected override void OnRefreshData()
        {
            Arrange();
        }

        private void Update()
        {
            float multiplier = _owner.Status.GetStatus(StatusType.AbilitySpeed, StatusSubType.Multiply);
            float speed = _speed + (_speed * multiplier);
            transform.Rotate(Vector3.back * speed * Time.deltaTime);
        }

        private void Arrange()
        {
            for (int i = 0; i < _count; ++i)
            {
                Transform shovel;
                if (i < transform.childCount)
                    shovel = transform.GetChild(i);
                else
                    shovel = Instantiate(_pref, transform).transform;

                shovel.localRotation = Quaternion.identity;
                shovel.localPosition = Vector3.zero;

                Vector3 rot = Vector3.forward * (360 * i / _count);
                shovel.Rotate(rot);
                shovel.Translate(shovel.up * 1.2f, Space.World);

                Melee spinShovel = shovel.GetComponent<Melee>();
                spinShovel.Init(_damage);
            }
        }
    }
}
