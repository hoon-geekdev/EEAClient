using EEA.AbilitySystem;
using EEA.Object;
using EEA.SOData;
using EEA.UI;
using EEA.Manager;
using UnityEngine;

namespace EEA.Manager
{
    public class GameManager : SingletonMono<GameManager>
    {
        [SerializeField] private GameObject _levelUpPopupPref;
        [SerializeField] private SOPlayerData _data;

        private Canvas _canvas;

        private int _level;
        private int _killCount;
        private int _exp;

        private Player _player;


        private float _time;
        private int[] LevelUpExp = {10, 30, 60, 100, 150, 210, 280, 360, 450, 550 };

        public Player Player => _player;
        public Inventory Inventory => Player.Inventory;
        public InventorySessionAbility InventorySessionAbility => Player.InventorySessionAbility;
        public int Level => _level;
        public int KillCount => _killCount;
        public int Exp => _exp;
        public int MaxExp => LevelUpExp[_level];
        public float GameTime => _time;

        protected override void OnAwake()
        {
            // Player tag로 찾기
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            _canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();

            _player.Init(500, 3f);
        }

        private void Start()
        {
            for (int i = 0; i < _data.SessionAbilities.Length; ++i)
            {
                InventorySessionAbility.AddItem(_data.SessionAbilities[i], i, 0);
            }

            // 임시 코드
            int abilityIdx = 0;
            int level = 4;
            SessionAbilityItem ability = InventorySessionAbility.GetItem(abilityIdx);
            InventorySessionAbility.AddItem(ability.Data, abilityIdx, level);

            Player.AddOrLevelUpSessionAbility(abilityIdx);
            // 임시 코드 엔드
        }

        private void Update()
        {
            _time += Time.deltaTime;
        }

        public void AddKillCount()
        {
            _killCount++;
            AddExp(1);
        }

        public void AddExp(int exp)
        {
            if (_level >= LevelUpExp.Length - 1)
                return;

            _exp += exp;
            if (_exp >= LevelUpExp[_level])
            {
                _exp -= LevelUpExp[_level];
                _level++;

                // 레벨업 팝업
                UILevelUpPopup _levelUpPopup = Instantiate(_levelUpPopupPref, _canvas.transform).GetComponent<UILevelUpPopup>();
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
