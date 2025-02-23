using EEA.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EEA.Manager
{
    public class PoolManager : SingletonMono<PoolManager>
    {
        [SerializeField] private GameObject[] _objectPrefabs;
        [SerializeField] private GameObject[] _effectPrefabs;

        private List<GameObject>[] _objectPool;
        private List<GameObject>[] _effectPool;

        protected override void OnAwake()
        {
            base.OnAwake();
            _objectPool = new List<GameObject>[_objectPrefabs.Length];
            for (int i = 0; i < _objectPrefabs.Length; i++)
            {
                _objectPool[i] = new List<GameObject>();
            }

            _effectPool = new List<GameObject>[_effectPrefabs.Length];
            for (int i = 0; i < _effectPrefabs.Length; i++)
            {
                _effectPool[i] = new List<GameObject>();
            }
        }

        public GameObject GetObject(int idx)
        {
            for (int i = 0; i < _objectPool[idx].Count; i++)
            {
                if (_objectPool[idx][i].activeSelf == false)
                {
                    _objectPool[idx][i].SetActive(true);
                    return _objectPool[idx][i];
                }
            }

            GameObject obj = Instantiate(_objectPrefabs[idx]);
            _objectPool[idx].Add(obj);
            return obj;
        }

        public GameObject GetEffect(int idx)
        {
            for (int i = 0; i < _effectPool[idx].Count; i++)
            {
                if (_effectPool[idx][i].activeSelf == false)
                {
                    _effectPool[idx][i].SetActive(true);
                    return _effectPool[idx][i];
                }
            }
            GameObject obj = Instantiate(_effectPrefabs[idx]);
            _effectPool[idx].Add(obj);
            return obj;
        }
    }
}
