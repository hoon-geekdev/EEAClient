using EEA.AbilitySystem;
using EEA.Object;
using EEA.UI;
using EEA.Manager;
using UnityEngine;
using EEA.Define;
using TableData;
using System.Collections.Generic;
using EEA.Utils;
using Cinemachine;

namespace EEA.Manager
{
    public class GameManager : SingletonMono<GameManager>
    {
        public Player Player => _player;
        public Inventory Inventory => Player.Inventory;
        public InventorySessionAbility InventorySessionAbility => Player.InventorySessionAbility;
        public int Level => _level;
        public int KillCount => _killCount;
        public float GameTime => _time;
        public Material SharedHitMaterial => _sharedHitMaterial;

        private int _level;
        private int _killCount;
        private int _exp;

        private Player _player;

        private float _time;
        private List<LevelTable> _levelDatas;
        private CameraShake _cameraShake;

        private Material _sharedHitMaterial;

        protected override void OnAwake()
        {
            Shader flashShader = Shader.Find("Custom/Sprite-Lit-Default-Custom");
            _sharedHitMaterial = new Material(flashShader);
            _sharedHitMaterial.SetFloat("_HitEffect", 0.7f);

            // Player tag로 찾기
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            _player.Init(100, 2f);

            CinemachineVirtualCamera virtualCamera = GameObject.FindGameObjectWithTag("vCamCharacterFollow").GetComponent<CinemachineVirtualCamera>();
            _cameraShake = virtualCamera.GetComponent<CameraShake>();

            List<LevelTable> tables = TableManager.Instance.GetDataList<LevelTable>();
            // tables에서 Type이 1인 데이터만 가져오기
            _levelDatas = tables.Count > 0 ? tables.FindAll(x => x.Type == 1) : new List<LevelTable>();
        }

        private void Start()
        {
            //for (int i = 0; i < _data.SessionAbilities.Length; ++i)
            //{
            //    InventorySessionAbility.AddItem(_data.SessionAbilities[i], i, 0);
            //}

            //// 임시 코드
            //int abilityIdx = 5;
            //int level = 4;
            //SessionAbilityItem ability = InventorySessionAbility.GetItem(abilityIdx);
            //InventorySessionAbility.AddItem(ability.Data, abilityIdx, level);

            //Player.AddOrLevelUpSessionAbility(abilityIdx);
            // 임시 코드 엔드
        }

        private void Update()
        {
            _time += Time.deltaTime;
        }
        
        public void ShakeCamera()
        {
            _cameraShake.TriggerShake();
        }

        public void InitUserData()
        {
            _level = 0;
            _killCount = 0;
            _exp = 0;

            InventorySessionAbility.AddData(15000002, 16000016);
            InventorySessionAbility.AddData(15000003, 16000031);
            InventorySessionAbility.AddData(15000004, 16000046);
            //InventorySessionAbility.AddData(15000009, 16000113);
            //InventorySessionAbility.AddData(15000010, 16000132);
            //Player.AddOrLevelUpSessionAbility(15000009);
            //Player.AddOrLevelUpSessionAbility(15000010);
            Player.AddOrLevelUpSessionAbility(15000002);
            Player.AddOrLevelUpSessionAbility(15000003);
            Player.AddOrLevelUpSessionAbility(15000004);
        }

        public void AddKillCount()
        {
            _killCount++;
            //AddExp(1);
        }

        public int GetNeedExp()
        {
            if (_level >= _levelDatas.Count)
                return 0;

            return _levelDatas[_level].Need_exp;
        }

        public int GetPrevLevelExp()
        {
            if (_level >= _levelDatas.Count)
                return 0;

            return _levelDatas[_level].Prev_exp;
        }

        public int GetCurExp()
        {
            return _exp;
        }

        public void AddExp(int exp)
        {
            if (_levelDatas.Count <= _level)
                return;

            _exp += exp;
            if (_exp >= _levelDatas[_level].Need_exp)
            {
                _level++;

                // 레벨업 팝업
                //UIAbilitySelectPopup _levelUpPopup = Instantiate(_levelUpPopupPref, _canvas.transform).GetComponent<UIAbilitySelectPopup>();
                UIManager.Instance.CreateUI<UIAbilitySelectPopup>(AssetPathUI.UIAbilitySelectPopup);
            }
        }

        public void GameStop()
        {
            Time.timeScale = 0;
        }

        public void GameResume()
        {
            Time.timeScale = 1;
        }
    }
}
