using CoreGame.FightSystem;
using CoreGame.FightSystem.Abilities;
using CoreGame.FightSystem.Models;
using FightSystem.Abilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreGame.FightSystem.AI
{
    [CreateAssetMenu(menuName = "CoreGame/Fight System/AI/SimpleMob")]
    public class SimpleMobAI : ScriptableAI
    {
        [SerializeField, Range(0f, 1f)] private float _guardChance = 0.1f;
        [SerializeField, Range(0f, 1f)] private float _skillChance = 0.05f;
        [SerializeField] private AbilityType _preferredSkillType = AbilityType.Attack;

        public override AIDecision DecideAction(
            IFightComponent selfComponent,
            IFightComponent targetComponent,
            FightCharacter selfCharacter,
            IReadOnlyList<AbilityEntity> availableAbilities,
            float currentEnergy)
        {
            float targetHealth = targetComponent.HealthComponent.CurrentHealth;
            float selfDamage = selfCharacter.Damage;

            if (targetHealth <= selfDamage)
            {
                return AIDecision.Simple(PlayerAction.Attack);
            }

            if (Random.value < _skillChance)
            {
                var skillAbility = availableAbilities.FirstOrDefault(e =>
                    e.ReferenceAbility.GetAbilityType() == _preferredSkillType &&
                    e.CurrentCooldown == 0 &&
                    e.ReferenceAbility.Cost <= currentEnergy);

                if (skillAbility != null)
                {
                    return new AIDecision(PlayerAction.UseSkill, skillAbility.ReferenceAbility);
                }
            }

            if (Random.value < _guardChance)
            {
                return AIDecision.Simple(PlayerAction.Guard);
            }

            return AIDecision.Simple(PlayerAction.Attack);
        }
    }
}