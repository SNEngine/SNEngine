using CoreGame.FightSystem;
using UnityEngine;

namespace CoreGame.FightSystem.Abilities
{
    public abstract class ScriptableOverTurnAbility : ScriptableAbility
    {
        [SerializeField, Min(0)] private int _turnsToTick = 3;

        public int TurnsToTick => _turnsToTick;

        protected abstract void TurnTick(IFightComponent user, IFightComponent target);

        public void ExecuteTurnTick(IFightComponent user, IFightComponent target)
        {
            TurnTick(user, target);
        }

        protected override void Execute(IFightComponent user, IFightComponent target)
        {
            TurnTick(user, target);
        }
    }
}