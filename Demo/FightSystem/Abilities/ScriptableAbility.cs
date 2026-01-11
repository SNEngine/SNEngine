using FightSystem.Abilities;
using SNEngine;
using UnityEngine;

namespace CoreGame.FightSystem.Abilities
{
    public abstract class ScriptableAbility : ScriptableObjectIdentity
    {
        [SerializeField, Min(0)] private int _cost = 1;
        [SerializeField, Min(0)] private float _duration;
        [SerializeField] private int _cooldown = 0;
        [SerializeField] private string _nameAbility;
        [SerializeField, TextArea] private string _descriptionAbility;

        public int Cost => _cost;
        public float Duration => _duration;
        public int Cooldown => _cooldown;
        public string NameAbility => _nameAbility;
        public string DescriptionAbility => _descriptionAbility;


        public void ExecuteEffect(IFightComponent user, IFightComponent target)
        {
            Execute(user, target);
        }

        protected abstract void Execute(IFightComponent user, IFightComponent target);

        public abstract AbilityType GetAbilityType();
    }
}