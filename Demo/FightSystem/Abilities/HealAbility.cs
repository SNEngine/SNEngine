using FightSystem.Abilities;
using UnityEngine;

namespace CoreGame.FightSystem.Abilities.NaotoAbilites
{
    [CreateAssetMenu(fileName = "NaotoHealAbility", menuName = "CoreGame/Fight System/Abilities/Heal")]
    public class HealAbility : ScriptableAbility
    {
        [SerializeField, Min(1)]
        private int _healAmount = 10;

        protected override void Execute(IFightComponent player, IFightComponent enemy)
        {
            player.HealthComponent.Heal(_healAmount);
        }

        public override AbilityType GetAbilityType()
        {
            return AbilityType.Skill;
        }
    }
}