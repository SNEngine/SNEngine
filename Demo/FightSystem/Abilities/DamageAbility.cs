using FightSystem.Abilities;
using CoreGame.FightSystem.Utils;
using UnityEngine;
using CoreGame.FightSystem;

namespace CoreGame.FightSystem.Abilities.NaotoAbilites
{
    [CreateAssetMenu(fileName = "NaotoDamageAbility", menuName = "CoreGame/Fight System/Abilities/Damage")]
    public class DamageAbility : ScriptableAbility
    {
        [SerializeField, Min(1)]
        private int _damageAmount = 10;

        protected override void Execute(IFightComponent player, IFightComponent enemy)
        {
            var targetCharacter = enemy.FightCharacter;
            float finalDamage = DamageUtils.ApplyGuardReduction(targetCharacter, _damageAmount);

            enemy.HealthComponent.TakeDamage(finalDamage);
        }

        public override AbilityType GetAbilityType()
        {
            return AbilityType.Skill;
        }
    }
}
