using System;
using System.Collections.Generic;

namespace CoreGame.FightSystem.Models
{
    [Serializable]
    public class AbilitySaveData
    {
        public string AbilityGUID { get; set; }
        public int CurrentCooldown { get; set; }
        public float RemainingTicks { get; set; }
    }
}