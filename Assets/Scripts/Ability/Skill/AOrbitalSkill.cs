using EEA.Manager;
using EEA.Object;
using System.Collections;
using UnityEngine;

namespace EEA.AbilitySystem
{
    public class AOrbitalSkill : Ability
    {
        protected override void OnAwake()
        {
            StartCoroutine(Effect());
        }

        protected override void OnRefreshData()
        {
            Arrange();
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
                {
                    trf = transform.GetChild(i);
                }
                else
                {
                    trf = PoolManager.Instance.GetObject(_tableData.Asset_path_unit).transform;
                    trf.SetParent(transform);
                }


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
