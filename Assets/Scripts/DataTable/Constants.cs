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
    }
}
