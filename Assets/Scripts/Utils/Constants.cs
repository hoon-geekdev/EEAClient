using EEA.AbilitySystem;
using EEA.Object;
using System.Transactions;
using TableData;
using UnityEngine;

namespace EEA.Define
{
    public class EditorPrefPathKey
    {
        public const string EnvPrefixKey = "EEA-EnvPrefix";
        public const string EnvPrefixesKey = "EEA-EnvPrefixes";
        public const string BinOutputFolderPathkey = "EEA-BinOutputFolderPath";
        public const string ExcelFolderPathkey = "EEA-ExcelFolderPath";
        public const string ClassOutputFolderPathkey = "EEA-ClassOutputFolderPath";
        public const string JsonOutputFolderPathkey = "EEA-JsonOutputFolderPath";
        public const string ConverterFilePathkey = "EEA-ConverterPath";
        public const string AdminIp = "EEA-AdminIp";
        public const string ServerRootPath = "EEA-ServerRootPath";
    }

    public class AssetPathObject
    {
        public const string Projectile = "Prefab/Projectile/BulletHandGun.prefab";
        public const string DefaultProjectile = "Prefab/Projectile/DefaultProjectile.prefab";
        public const string BulletCase = "Prefab/Projectile/Bullet Case.prefab";
        public const string FreezeHitEffect = "Prefab/Skill/Snow hit.prefab";
        public const string EnemyTest = "Prefab/Object/Enemy/MaskTint03.prefab";
        public const string HitEffect = "Effect/Prefab/Particle_hit.prefab";
        public const string AssistRobot = "Object/GuideBot/Sci-FiBallRobot/BallRobot_Blue.prefab";
        public const string MonsterSpawnEffect = "Effect/Prefab/Effect_monster/Effect_Spawn_Monser.prefab";
        public const string UICommandEffectCircle = "Effect/Prefab/UI_effect/UICommandEffectCircle.prefab";
    }

    public class AssetPathVFX
    {
        public const string DefaultHit = "VFX/Electro hit.prefab";
    }

    public class AssetPathScene
    {
        public const string Intro = "IntroScene";
        public const string Lobby = "Scene/LobbyScene.unity";
        public const string Stage = "Scene/StageScene.unity";
    }

    public class AssetPathSound
    {
        public const string DefaultTouch = "Sound/SFX/default_touch.ogg";
        public const string DefaultPerfect = "Sound/SFX/perfect.ogg";
        public const string DefaultQte = "Sound/SFX/qte.ogg";
        public const string DefaultSkill = "Sound/SFX/skill.ogg";
        public const string LobbyBgm = "Sound/BGM/bgm_lobby.mp3";
        public const string AndroidHit = "Sound/SFX/android_hit.ogg";
        public const string MonsterHit = "Sound/SFX/Hit.ogg";
    }

    public class AssetPathUI
    {
        // built-in
        public const string UIIntro = "BuiltIn/UIIntro/UIIntro";

        public const string UILoading = "UI/UILoading/UILoading.prefab";
        public const string UILobby = "UI/UILobby/UILobby.prefab";
        public const string UIStageHUD = "UI/UIStageHUD/UIStageHUD.prefab";
        public const string UIAbilitySelectPopup = "UI/UIAbilitySelectPopup/UIAbilitySelectPopup.prefab";
        public const string UIAbilityItem = "UI/UIAbilitySelectPopup/UIAbilityItem.prefab";
        public const string UIJoystick = "UI/UIJoystick/UIJoystick.prefab";
        public const string UIDamageText = "UI/UIDamageText/UIDamageText.prefab";
        public const string UIMonsterHealth = "UI/UIMonsterHP/UIMonsterHealth.prefab";
        public const string UISystemPopup = "UI/UISystemPopup/UISystemPopup.prefab";
        public const string UIInventory = "UI/UIInventory/UIInventory.prefab";
    }

    // 기본 데미지 이벤트 클래스
    public class DamageEvent
    {
        public float _damage;
        public float _range;
        public float _delay;
        public float _speed;
        public float _duration;
        public int _penetration;
        public int _count;
        public float _tick;
        public AbilityTable _tableData;
        public Transform _target;
        public ObjectBase _owner;

        public DamageEvent()
        {
            Reset();
        }

        // 값 초기화 메서드
        public virtual void Reset()
        {
            _damage = 0;
            _range = 0;
            _delay = 0;
            _speed = 0;
            _duration = 0;
            _penetration = 0;
            _count = 0;
            _tick = 0;
            _tableData = null;
            _target = null;
            _owner = null;
        }

        // 데미지 값만 업데이트
        public DamageEvent SetDamage(float damage)
        {
            _damage = damage;
            return this;
        }

        // 소유자 설정
        public DamageEvent SetOwner(ObjectBase owner)
        {
            _owner = owner;
            return this;
        }

        // 타겟 설정
        public DamageEvent SetTarget(Transform target)
        {
            _target = target;
            return this;
        }

        // 스킬 테이블 설정
        public DamageEvent SetTableData(AbilityTable tableData)
        {
            _tableData = tableData;
            return this;
        }

        // 속도 설정
        public DamageEvent SetSpeed(float speed)
        {
            _speed = speed;
            return this;
        }

        // 관통 설정
        public DamageEvent SetPenetration(int penetration)
        {
            _penetration = penetration;
            return this;
        }

        // 범위 설정
        public DamageEvent SetRange(float range)
        {
            _range = range;
            return this;
        }

        // 간편하게 값 설정하기 위한 메서드
        public DamageEvent Setup(ObjectBase owner, float damage, AbilityTable tableData = null)
        {
            Reset();
            _owner = owner;
            _damage = damage;
            _tableData = tableData;
            return this;
        }
    }

    // 플레이어가 사용하는 데미지 이벤트
    public class PlayerDamageEvent : DamageEvent
    {
        // 플레이어 스킬 데미지 관련 추가 속성
        public float _criticalRate;     // 크리티컬 확률
        public float _criticalDamage;   // 크리티컬 데미지 배율
        public string _skillEffectPath; // 스킬 효과 경로
        public int _skillLevel;         // 스킬 레벨
        
        public PlayerDamageEvent()
        {
            Reset();
        }
        
        public override void Reset()
        {
            base.Reset();
            // 플레이어 공격은 기본적으로 관통 1
            _penetration = 1;
            _criticalRate = 0;
            _criticalDamage = 1.5f;
            _skillEffectPath = string.Empty;
            _skillLevel = 1;
        }

        // 크리티컬 설정
        public PlayerDamageEvent SetCritical(float rate, float damage)
        {
            _criticalRate = rate;
            _criticalDamage = damage;
            return this;
        }

        // 스킬 레벨 설정
        public PlayerDamageEvent SetSkillLevel(int level)
        {
            _skillLevel = level;
            return this;
        }
    }

    // 몬스터가 사용하는 데미지 이벤트
    public class EnemyDamageEvent : DamageEvent
    {
        // 몬스터 공격 관련 추가 속성
        public eMonsterType _monsterType;
        public bool _isAreaAttack;      // 범위 공격 여부
        
        public EnemyDamageEvent()
        {
            Reset();
        }
        
        public override void Reset()
        {
            base.Reset();
            // 몬스터 공격은 기본적으로 관통 없음
            _penetration = 0;
            _monsterType = eMonsterType.None;
            _isAreaAttack = false;
        }

        // 몬스터 타입 설정
        public EnemyDamageEvent SetMonsterType(eMonsterType type)
        {
            _monsterType = type;
            return this;
        }

        // 범위 공격 여부 설정
        public EnemyDamageEvent SetAreaAttack(bool isArea)
        {
            _isAreaAttack = isArea;
            return this;
        }
    }
}
