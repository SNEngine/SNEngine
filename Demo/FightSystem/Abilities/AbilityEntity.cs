using System;
using UnityEngine;

namespace CoreGame.FightSystem.Abilities
{
    [Serializable]
    public class AbilityEntity
    {

        public ScriptableAbility ReferenceAbility { get; private set; }
        public int CurrentCooldown { get; set; }

        [field: SerializeField] public float RemainingTicks { get; set; }


        public AbilityEntity(ScriptableAbility referenceAbility)
        {
            ReferenceAbility = referenceAbility;
            CurrentCooldown = 0;

            if (ReferenceAbility is ScriptableOverTurnAbility overTurnAbility)
            {
                RemainingTicks = overTurnAbility.TurnsToTick;
            }
            else
            {
                RemainingTicks = 0;
            }
        }
    }
}