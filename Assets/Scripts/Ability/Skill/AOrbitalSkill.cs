using EEA.Object;
using System.Collections;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class AOrbitalSkill : Ability
    {
        private float _duration = 3f;
        protected override void OnRefreshData()
        {
            Arrange();
        }

        protected override void OnStart()
        {
            StartCoroutine(Effect());
        }

        private IEnumerator Effect()
        {
            while(true)
            {
                EnableChild(true);
                yield return new WaitForSeconds(_duration);

                EnableChild(false);
                yield return new WaitForSeconds(_delay);
            }
        }

        private void EnableChild(bool enable)
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                transform.GetChild(i).gameObject.SetActive(enable);
            }
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
                Transform trf;
                if (i < transform.childCount)
                    trf = transform.GetChild(i);
                else
                    trf = Instantiate(_unitPref, transform).transform;

                trf.localRotation = Quaternion.identity;
                trf.localPosition = Vector3.zero;

                Vector3 rot = Vector3.forward * (360 * i / _count);
                trf.Rotate(rot);
                trf.Translate(trf.up * 1.2f, Space.World);

                AOrbitalUnit unit = trf.GetComponent<AOrbitalUnit>();
                unit.Init(_damage);
            }
        }
    }
}
