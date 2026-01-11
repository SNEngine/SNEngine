using System;

namespace SNEngine.Localization.Models
{
    [Serializable]
    public class NodeLocalizationData
    {
        public string GUID {  get; set; }
        public object Value { get; set; }
    }
}
