using FightSystem.Abilities;
using UnityEngine;
using CoreGame.FightSystem;
using CoreGame.FightSystem.Utils; // <-- Добавляем для доступа к DamageUtils

namespace CoreGame.FightSystem.Abilities
{
    [CreateAssetMenu(fileName = "SuccubusDrainAbility", menuName = "CoreGame/Fight System/Abilities/Succubus Drain")]
    public class SuccubusDrainAbility : ScriptableOverTurnAbility
    {
        [SerializeField, Range(0.01f, 1.0f)]
        private float _drainPercent = 0.05f;

        protected override void TurnTick(IFightComponent user, IFightComponent target)
        {
            float targetMaxHealth = target.HealthComponent.MaxHealth;
            float drainAmount = targetMaxHealth * _drainPercent;

            if (drainAmount > 0)
            {
                float finalDrainAmount = DamageUtils.ApplyGuardReduction(target.FightCharacter, drainAmount);


                target.HealthComponent.TakeDamage(finalDrainAmount);
                user.HealthComponent.Heal(finalDrainAmount);
            }
        }

        public override AbilityType GetAbilityType()
        {
            return AbilityType.Skill;
        }
    }
}