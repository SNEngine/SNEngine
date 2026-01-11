using SNEngine.CharacterSystem;
using System;

namespace SNEngine.Localization.Models
{
    [Serializable]
    public class CharacterLocalizationData
    {
        public string GUID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public CharacterLocalizationData() { }
        public CharacterLocalizationData (Character character)
        {
            GUID = character.GUID;
            Name = character.OriginalName;
            Description = character.Description;
        }
    }
}
