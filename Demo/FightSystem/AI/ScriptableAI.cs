using CoreGame.FightSystem;
using CoreGame.FightSystem.Abilities;
using CoreGame.FightSystem.Models;
using SNEngine;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame.FightSystem.AI
{
    public abstract class ScriptableAI : ScriptableObjectIdentity
    {
        public abstract AIDecision DecideAction(
            IFightComponent selfComponent,
            IFightComponent targetComponent,
            FightCharacter selfCharacter,
            IReadOnlyList<AbilityEntity> availableAbilities,
            float currentEnergy);
    }
}