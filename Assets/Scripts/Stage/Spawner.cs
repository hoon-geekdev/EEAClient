using EEA.Manager;
using EEA.Object;
using System;
using System.Collections;
using UnityEngine;

namespace EEA.Stage
{
    public class Spawner : MonoBehaviour
    {
        private Player _player;
        private SpawnTable[] _spawnTables;

        private void Start()
        {
            _player = GameManager.Instance.Player;
            _spawnTables = new SpawnTable[]
            {
                new SpawnTable { ObjType = 0, Health = 30, Speed = 1 },
                new SpawnTable { ObjType = 1, Health = 60, Speed = 2 },
            };
            StartCoroutine(Spawn());
        }

        private IEnumerator Spawn()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                int rnd = UnityEngine.Random.Range(0, 2);
                GameObject obj = PoolManager.Instance.GetObject(rnd);
                obj.GetComponent<Enemy>().Init(_spawnTables[rnd].Health, _spawnTables[rnd].Speed, rnd);

                // player 위치에서 랜덤한 위치로 생성, 최소 거리는 10
                Vector2 playerPos = _player.transform.position;
                Vector2 spawnPos = playerPos + UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(10f, 20f);

                obj.transform.position = spawnPos;
            }
        }
    }

    [Serializable]
    public class SpawnTable
    {
        public int ObjType;
        public int Health;
        public float SpawnTime;
        public float Speed;
    }
}
