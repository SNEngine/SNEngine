using CoreGame.FightSystem.Abilities;
using CoreGame.FightSystem.Models;
using System;
using System.Collections.Generic;

namespace CoreGame.FightSystem.AI
{
    [Serializable]
    public class AIEntity
    {
        public ScriptableAI ReferenceAI { get; private set; }

        public AIEntity(ScriptableAI referenceAI)
        {
            ReferenceAI = referenceAI;
        }

        public AIDecision DecideAction(
            IFightComponent selfComponent,
            IFightComponent targetComponent,
            FightCharacter selfCharacter,
            IReadOnlyList<AbilityEntity> availableAbilities,
            float currentEnergy)
        {
            return ReferenceAI?.DecideAction(selfComponent, targetComponent, selfCharacter, availableAbilities, currentEnergy) ?? AIDecision.Simple(PlayerAction.Wait);
        }
    }
}