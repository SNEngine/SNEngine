using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SNEngine.Localization.Models
{
    [Serializable]
    public class LanguageManifest
    {
        public string Characters { get; set; }

        public string Ui { get; set; }

        public string Metadata { get; set; }

        public string Flag { get; set; }
        public List<string> Dialogues { get; set; }
    }
}
