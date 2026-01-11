using FightSystem.Abilities;
using UnityEngine;

namespace CoreGame.FightSystem.Abilities
{
    [CreateAssetMenu(fileName = "GuardAbility", menuName = "CoreGame/Fight System/Abilities/Guard Ability")]
    public class GuardAbility : ScriptableOverTurnAbility
    {

        protected override void TurnTick(IFightComponent user, IFightComponent target)
        {
            user.StartGuard();
        }

        public override AbilityType GetAbilityType()
        {
            return AbilityType.Skill;
        }
    }
}
