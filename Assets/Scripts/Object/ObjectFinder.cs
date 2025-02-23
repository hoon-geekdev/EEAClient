using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EEA.Object
{
    public class ObjectFinder : MonoBehaviour
    {
        [SerializeField] private float _scanRange;
        [SerializeField] private LayerMask _scanLayer;
        private RaycastHit2D[] _scanTargets;
        // 우선순위 큐를 활용한 가까운 타겟 순으로 정렬
        private SortedList<float, Transform> _sortedTargets;

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
            while (true)
            {
                _scanTargets = Physics2D.CircleCastAll(transform.position, _scanRange, Vector2.zero, 0, _scanLayer);
                if (_scanTargets.Length > 0)
                    _sortedTargets = GetNearest();
                yield return new WaitForSeconds(0.1f);
            }
        }

        public Transform PeakNearTarget()
        {
            if (_sortedTargets == null)
                return null;

            if (_sortedTargets.Count > 0)
            {
                Transform target = _sortedTargets.Values[0];
                _sortedTargets.RemoveAt(0);
                return target;
            }
            
            return null;
        }

        SortedList<float, Transform> GetNearest()
        {
            SortedList<float, Transform> sortedTargets = new SortedList<float, Transform>();
            int i = 0;
            foreach (RaycastHit2D hit in _scanTargets)
            {
                if (i > 30)
                    break;

                i++;
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                if (!sortedTargets.ContainsKey(distance))
                {
                    sortedTargets.Add(distance, hit.transform);
                }
            }

            return sortedTargets;
        }
    }
}
