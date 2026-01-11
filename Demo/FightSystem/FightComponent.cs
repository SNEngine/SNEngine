using CoreGame.FightSystem.HealthSystem;
using CoreGame.FightSystem.ManaSystem;
using UnityEngine;

namespace CoreGame.FightSystem
{
    public class FightComponent : MonoBehaviour, IFightComponent
    {
        private HealthComponent _healthComponent;
        private ManaComponent _manaComponent;
        private FightCharacter _fightCharacter;

        public IHealthComponent HealthComponent => _healthComponent;
        public IManaComponent ManaComponent => _manaComponent;

        public bool IsGuarding { get; private set; }
        public FightCharacter FightCharacter => _fightCharacter;

        public void AddComponents()
        {
            _healthComponent = gameObject.AddComponent<HealthComponent>();
            _manaComponent = gameObject.AddComponent<ManaComponent>();
        }

        public void SetFightCharacter(FightCharacter character)
        {
            _fightCharacter = character;
        }

        public void StartGuard()
        {
            IsGuarding = true;
        }

        public void StopGuard()
        {
            IsGuarding = false;
        }
    }
}
