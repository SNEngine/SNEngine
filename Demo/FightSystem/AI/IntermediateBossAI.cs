using CoreGame.FightSystem;
using CoreGame.FightSystem.Abilities;
using CoreGame.FightSystem.Models;
using FightSystem.Abilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoreGame.FightSystem.AI
{
    [CreateAssetMenu(menuName = "CoreGame/Fight System/AI/IntermediateBoss")]
    public class IntermediateBossAI : ScriptableAI
    {
        [SerializeField, Range(0f, 1f)] private float _lowHPThreshold = 0.5f;
        [SerializeField, Range(0f, 1f)] private float _healThresholdLowHP = 0.3f;
        [SerializeField] private AbilityType _signatureSkillType = AbilityType.Attack;
        [SerializeField] private float _attackWeightHighHP = 70f;
        [SerializeField] private float _attackWeightLowHP = 40f;
        [SerializeField] private float _guardWeightLowHP = 40f;
        [SerializeField] private float _skillWeightHighHP = 30f;
        [SerializeField] private float _skillWeightLowHP = 30f;

        public override AIDecision DecideAction(
            IFightComponent selfComponent,
            IFightComponent targetComponent,
            FightCharacter selfCharacter,
            IReadOnlyList<AbilityEntity> availableAbilities,
            float currentEnergy)
        {
            float selfHealthRatio = selfComponent.HealthComponent.CurrentHealth / selfComponent.HealthComponent.MaxHealth;
            float targetHealth = targetComponent.HealthComponent.CurrentHealth;
            float selfDamage = selfCharacter.Damage;

            // Проверка добивания
            if (targetHealth <= selfDamage)
            {
                return AIDecision.Simple(PlayerAction.Attack);
            }

            // Проверка хила на критическом HP
            if (selfHealthRatio <= _healThresholdLowHP)
            {
                var healDecision = ChooseSkill(availableAbilities, currentEnergy, AbilityType.Heal);
            }

            // Попытка использовать сигнатурный скилл
            var signatureDecision = ChooseSkill(availableAbilities, currentEnergy, _signatureSkillType);

            // Решение по весам в зависимости от HP
            if (selfHealthRatio <= _lowHPThreshold)
            {
                return WeightedDecision(
                    availableAbilities,
                    currentEnergy,
                    _attackWeightLowHP,
                    _guardWeightLowHP,
                    _skillWeightLowHP);
            }
            else
            {
                return WeightedDecision(
                    availableAbilities,
                    currentEnergy,
                    _attackWeightHighHP,
                    0f,
                    _skillWeightHighHP);
            }
        }

        private AIDecision ChooseSkill(IReadOnlyList<AbilityEntity> availableAbilities, float currentEnergy, AbilityType type)
        {
            var skills = availableAbilities
                .Where(a => a.CurrentCooldown == 0 &&
                            a.ReferenceAbility.Cost <= currentEnergy &&
                            a.ReferenceAbility.GetAbilityType() == type)
                .ToList();

            if (skills.Count == 0)
                return AIDecision.Simple(PlayerAction.Attack);

            int idx = Random.Range(0, skills.Count);
            return new AIDecision(PlayerAction.UseSkill, skills[idx].ReferenceAbility);
        }

        private AIDecision WeightedDecision(IReadOnlyList<AbilityEntity> availableAbilities, float currentEnergy,
            float attackWeight, float guardWeight, float skillWeight)
        {
            var skills = availableAbilities
                .Where(a => a.CurrentCooldown == 0 && a.ReferenceAbility.Cost <= currentEnergy)
                .ToList();

            AbilityEntity chosenSkill = null;
            if (skills.Count > 0)
            {
                chosenSkill = skills[Random.Range(0, skills.Count)];
            }
            else
            {
                attackWeight += skillWeight;
                skillWeight = 0f;
            }

            float total = attackWeight + guardWeight + skillWeight;
            if (total <= 0f) return AIDecision.Simple(PlayerAction.Attack);

            float roll = Random.Range(0f, total);

            if (roll < attackWeight) return AIDecision.Simple(PlayerAction.Attack);
            if (roll < attackWeight + guardWeight) return AIDecision.Simple(PlayerAction.Guard);
            if (chosenSkill != null && roll < attackWeight + guardWeight + skillWeight)
                return new AIDecision(PlayerAction.UseSkill, chosenSkill.ReferenceAbility);

            return AIDecision.Simple(PlayerAction.Attack);
        }
    }
}
