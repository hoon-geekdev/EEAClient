using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EEA.Object
{
    public class ObjectFinder : MonoBehaviour
    {
        [SerializeField] private float _scanRange;
        [SerializeField] private LayerMask _scanLayer;
        private List<RaycastHit2D> _scanTargets;
        // 우선순위 큐를 활용한 가까운 타겟 순으로 정렬

        //private void FixedUpdate()
        //{
        //    _scanTargets = Physics2D.CircleCastAll(transform.position, _scanRange, Vector2.zero, 0, _scanLayer);

        //    if (_scanTargets.Length > 0)
        //        _sortedTargets = GetNearest();
        //}

        private void OnEnable()
        {
            StartCoroutine(Scan());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator Scan()
        {
            yield return new WaitForSeconds(1f);

            while (true)
            {
                _scanTargets = Physics2D.CircleCastAll(transform.position, _scanRange, Vector2.zero, 0, _scanLayer).ToList();
                SortTargets();
                yield return new WaitForSeconds(0.1f);
            }
        }

        public Transform PeakNearTarget()
        {
            if (_scanTargets == null || _scanTargets.Count <= 0)
                return null;

            Transform target = _scanTargets[0].transform;
            _scanTargets.RemoveAt(0);
            return target;
        }

        public Transform PeakRandomTarget()
        {
            if (_scanTargets == null || _scanTargets.Count <= 0)
                return null;

            int index = Random.Range(0, _scanTargets.Count);
            Transform target = _scanTargets[index].transform;
            _scanTargets.RemoveAt(index);
            return target;
        }

        private void SortTargets()
        {
            if (_scanTargets == null || _scanTargets.Count <= 0)
                return;

            _scanTargets.Sort((a, b) =>
            {
                float distA = Vector2.Distance(transform.position, a.transform.position);
                float distB = Vector2.Distance(transform.position, b.transform.position);
                return distA.CompareTo(distB);
            });
        }
    }
}
