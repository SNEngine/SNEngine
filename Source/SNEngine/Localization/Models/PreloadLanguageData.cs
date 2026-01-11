using System;

namespace SNEngine.Localization.Models
{
    [Serializable]
    public class PreloadLanguageData
    {
        public string PathFlag { get; set; }
        public string CodeLanguage { get; set; }
        public LanguageMetaData MetaData { get; set; }
    }
}
