using System;
using System.Collections.Generic;

namespace SNEngine.Localization.Models
{
    [Serializable]
    public class AvailableLanguagesManifest
    {
        public List<LanguageEntry> Languages { get; set; }
    }
}
