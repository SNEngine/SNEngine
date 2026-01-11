using System;

namespace SNEngine.Localization.Models
{
    [Serializable]
    public class LanguageMetaData
    {
        public string NameLanguage { get; set; }
        public string Author { get; set; }
        public uint Version { get; set; }
    }
}
