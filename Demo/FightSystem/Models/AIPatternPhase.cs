using FightSystem.Abilities;
using System;
using UnityEngine;

namespace CoreGame.FightSystem.Models
{
    [Serializable]
    public class AIPatternPhase
    {
        [SerializeField] private string _phaseName;
        [SerializeField] private float _healthThreshold = 1f;
        [SerializeField] private float _damageMultiplier = 1f;
        [SerializeField] private float _urgentHealThreshold = 0.35f;
        [SerializeField] private float _manaBurnThreshold = 0.2f;
        [SerializeField] private int _energyWaitCostMin = 3;
        [SerializeField] private AbilityType _preferredSkillType = AbilityType.Attack;
        [SerializeField] private AbilityType _secondarySkillType = AbilityType.Heal;
        [SerializeField] private AITacticWeights _weights = new AITacticWeights();

        public float HealthThreshold => _healthThreshold;
        public float DamageMultiplier => _damageMultiplier;
        public float UrgentHealThreshold => _urgentHealThreshold;
        public float ManaBurnThreshold => _manaBurnThreshold;
        public int EnergyWaitCostMin => _energyWaitCostMin;
        public AbilityType PreferredSkillType => _preferredSkillType;
        public AbilityType SecondarySkillType => _secondarySkillType;
        public AITacticWeights Weights => _weights;
    }

}
