using UnityEngine;

namespace EEA.SOData
{
    [CreateAssetMenu(fileName = "SOPlayerData", menuName = "SO/PlayerData")]
    public class SOPlayerData : ScriptableObject
    {
        [SerializeField] private SOSessionAbility[] _sessionAbilites;

        public SOSessionAbility[] SessionAbilities => _sessionAbilites;
    }
}
