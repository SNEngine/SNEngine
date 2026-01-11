using CoreGame.FightSystem.Abilities;

namespace CoreGame.FightSystem.Models
{
    public struct AIDecision
    {
        public PlayerAction Action { get; private set; }
        public ScriptableAbility Ability { get; private set; }

        public AIDecision(PlayerAction action, ScriptableAbility ability = null)
        {
            Action = action;
            Ability = ability;
        }

        public static AIDecision Simple(PlayerAction action) => new AIDecision(action);
    }
}
