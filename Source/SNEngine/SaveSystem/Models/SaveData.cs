using System;
using System.Collections.Generic;
using System.Linq;

namespace SNEngine.SaveSystem.Models
{
    [Serializable]
    public class SaveData : IEquatable<SaveData>
    {
        public DateTime DateSave { get; set; }
        public string DialogueGUID { get; set; }
        public Dictionary<string, object> Variables { get; set; }
        public Dictionary<string, object> GlobalVariables { get; set; }
        public Dictionary<string, object> NodesData { get; set; }
        public string CurrentNode { get; set; }

        public bool Equals(SaveData other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return DialogueGUID == other.DialogueGUID &&
                   CurrentNode == other.CurrentNode &&
                   AreDictionariesEqual(Variables, other.Variables) &&
                   AreDictionariesEqual(GlobalVariables, other.GlobalVariables) &&
                   AreDictionariesEqual(NodesData, other.NodesData);
        }

        public override bool Equals(object obj) => Equals(obj as SaveData);

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(DialogueGUID);
            hash.Add(CurrentNode);
            return hash.ToHashCode();
        }

        private bool AreDictionariesEqual(Dictionary<string, object> d1, Dictionary<string, object> d2)
        {
            if (ReferenceEquals(d1, d2)) return true;
            if (d1 == null || d2 == null) return false;
            if (d1.Count != d2.Count) return false;

            foreach (var kvp in d1)
            {
                if (!d2.TryGetValue(kvp.Key, out object value)) return false;
                if (!Equals(kvp.Value, value)) return false;
            }
            return true;
        }
    }
}