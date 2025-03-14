using EEA.Object;
using System.Collections;
using System.Collections.Generic;
using TableData;
using UnityEngine;

namespace EEA.Manager
{
    public class StageManager : SingletonMono<StageManager>
    {
        private Player _player;
        private int _stageCode;
        private int _currentWaveIndex;
        public List<WaveData[]> _waveDatas = new List<WaveData[]>();

        public void SetStage(int stageCode)
        {
            _waveDatas.Clear();
            _stageCode = stageCode;
            _currentWaveIndex = 0;
            _player = GameManager.Instance.Player;

            StageTable stageData = TableManager.Instance.GetData<StageTable>(_stageCode);
            if (stageData == null)
            {
                Debug.LogError($"StageManager::SetStage() - stageData is null. stageCode: {_stageCode}");
                return;
            }

            int[][] waves = { stageData.Wave_1, stageData.Wave_2 };
            for (int i = 0; i < waves.Length; i++)
            {
                WaveData[] waveDatas = new WaveData[waves[i].Length];
                for (int j = 0; j < waves[i].Length; j++)
                {
                    StageWaveTable waveData = TableManager.Instance.GetData<StageWaveTable>(waves[i][j]);
                    if (waveData == null)
                    {
                        Debug.LogError($"StageManager::SetStage() - waveData is null. waveId: {waves[i][j]}");
                        continue;
                    }

                    WaveData wave = new WaveData();
                    wave._spawnTime = waveData.Time;
                    wave._enemyId = waveData.Monster_id;
                    wave._spawnMaxCount = waveData.Spawn_count;
                    wave._spawnCurCount = 0;
                    wave._spawnInterval = waveData.Spawn_interval;
                    wave._spawnAreaType = waveData.Spawn_area;
                    waveDatas[j] = wave;
                }

                _waveDatas.Add(waveDatas);
            }

            StartWave();
        }

        public void Update()
        {
            if (_waveDatas.Count == 0 || _currentWaveIndex >= _waveDatas.Count)
                return;

            bool isWaveEnd = true;
            for (int i = 0; i < _waveDatas[_currentWaveIndex].Length; i++)
            {
                if (_waveDatas[_currentWaveIndex][i]._spawnCurCount < _waveDatas[_currentWaveIndex][i]._spawnMaxCount)
                {
                    isWaveEnd = false;
                    break;
                }
            }

            if (isWaveEnd)
            {
                _currentWaveIndex++;
                StartWave();
            }
        }

        public void StartWave()
        {
            if (_waveDatas.Count == 0 || _currentWaveIndex >= _waveDatas.Count)
            {
                Debug.LogError("StageManager::StartWave() - _waveDatas is empty.");
                return;
            }

            for (int i = 0; i < _waveDatas[_currentWaveIndex].Length; i++)
            {
                WaveData waveData = _waveDatas[_currentWaveIndex][i];
                StartCoroutine(SpawnWave(waveData));
            }
        }

        private IEnumerator SpawnWave(WaveData waveData)
        {
            WaitForSeconds wait = new WaitForSeconds(waveData._spawnTime);
            yield return wait;

            ObjectTable objectData = TableManager.Instance.GetData<ObjectTable>(waveData._enemyId);
            if (objectData == null)
            {
                Debug.LogError($"StageManager::SpawnWave() - objectData is null. objectId: {waveData._enemyId}");
                yield break;
            }

            wait = new WaitForSeconds(waveData._spawnInterval);

            while (waveData._spawnCurCount < waveData._spawnMaxCount)
            {
                Vector2 playerPos = _player.transform.position;
                GameObject go = PoolManager.Instance.GetObject(objectData.Asset_path);
                Enemy enemy = go.GetComponent<Enemy>();
                enemy.Init(objectData.Code);

                switch(waveData._spawnAreaType)
                {
                    case "left":
                        go.transform.position = playerPos + new Vector2(-15f, Random.Range(-5f, 5f));
                        break;
                    case "right":
                        go.transform.position = playerPos + new Vector2(15f, Random.Range(-5f, 5f));
                        break;
                    case "top":
                        go.transform.position = playerPos + new Vector2(Random.Range(-10f, 10f), 5f);
                        break;
                    case "bottom":
                        go.transform.position = playerPos + new Vector2(Random.Range(-10f, 10f), -5f);
                        break;
                    case "circle":
                        float angle = (360f / waveData._spawnMaxCount) * waveData._spawnCurCount;
                        float radian = angle * Mathf.Deg2Rad; // 도(degree) → 라디안(radian) 변환

                        go.transform.position = playerPos + new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * 10;
                        break;
                    case "Random":
                    default:
                        //Vector3 pos = playerPos + new Vector2(Random.Range(-10f, 10f), Random.Range(-5f, 5f));
                        //go.transform.position = pos;
                        go.transform.position = playerPos + Random.insideUnitCircle.normalized * Random.Range(10f, 20f);
                        break;
                }

                waveData._spawnCurCount++;
                yield return wait;
            }
        }
    }

    public class WaveData 
    { 
        public float _spawnTime;
        public int _enemyId;
        public int _spawnMaxCount;
        public int _spawnCurCount;
        public float _spawnInterval;
        public string _spawnAreaType;
    }
}
