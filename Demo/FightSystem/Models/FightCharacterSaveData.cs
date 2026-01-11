using System;
using System.Collections.Generic;

namespace CoreGame.FightSystem.Models
{
    [Serializable]
    public class FightCharacterSaveData
    {
        public float CurrentHealth { get; set; }
        public float CurrentEnergy { get; set; }
        public List<AbilitySaveData> AbilitiesData { get; set; }
        public List<AbilitySaveData> ActiveTickEffects { get; set; }

        public FightCharacterSaveData()
        {
            AbilitiesData = new List<AbilitySaveData>();
            ActiveTickEffects = new List<AbilitySaveData>();
        }
    }
}