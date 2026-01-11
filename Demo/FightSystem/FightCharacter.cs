using CoreGame.FightSystem.Abilities;
using CoreGame.FightSystem.AI;
using SNEngine;
using SNEngine.CharacterSystem;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame.FightSystem
{
    [CreateAssetMenu(menuName = "CoreGame/Fight System/New Fight Character")]
    public class FightCharacter : ScriptableObjectIdentity
    {
        [SerializeField] private Character _referenceCharacter;
        [SerializeField, Min(1)] private float _damage = 1;
        [SerializeField, Min(1)] private float _health = 100;
        [SerializeField, Range(0, 1)] private float _guardReductionPercentage = 0.5f;
        [SerializeField, Range(0, 1)] private float _criticalHitChance = 0.1f;
        [SerializeField, Min(1)] private float _criticalHitMultiplier = 1.5f;
        [SerializeField, Min(1)] private int _energyPoint = 5;
        [SerializeField] private ScriptableAbility[] _abilities;
        [SerializeField] private ScriptableAI _referenceAI;
        [SerializeField] private Color _color = Color.white;
        [SerializeField] private Sprite _avatar;

        public Character ReferenceCharacter => _referenceCharacter;
        public float Damage => _damage;
        public float Health => _health;
        public float GuardReductionPercentage => _guardReductionPercentage;
        public float CriticalHitChance => _criticalHitChance;
        public float CriticalHitMultiplier => _criticalHitMultiplier;

        public int EnergyPoint => _energyPoint;

        public IEnumerable<ScriptableAbility> Abilities => _abilities;

        public ScriptableAI ReferenceAI => _referenceAI;

        public Color Color => _color;
        public Sprite Avatar => _avatar;
    }
}