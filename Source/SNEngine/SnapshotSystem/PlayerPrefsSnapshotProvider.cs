using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace SNEngine.SnapshotSystem
{
    public class PlayerPrefsSnapshotProvider : ISnapshotProvider
    {
        private readonly string _key;
        private List<string> _history;

        public PlayerPrefsSnapshotProvider(string saveName)
        {
            _key = $"SNE_Snapshot_{saveName}";
            string raw = PlayerPrefs.GetString(_key, "[]");
            _history = JsonConvert.DeserializeObject<List<string>>(raw) ?? new List<string>();
        }

        public UniTask AppendAsync(byte[] data)
        {
            _history.Add(Convert.ToBase64String(data));
            Save();
            return UniTask.CompletedTask;
        }

        public UniTask<byte[]> PopLastAsync()
        {
            if (_history.Count == 0) return UniTask.FromResult<byte[]>(null);
            string last = _history[^1];
            _history.RemoveAt(_history.Count - 1);
            Save();
            return UniTask.FromResult(Convert.FromBase64String(last));
        }

        public UniTask ClearAsync()
        {
            _history.Clear();
            PlayerPrefs.DeleteKey(_key);
            return UniTask.CompletedTask;
        }

        private void Save()
        {
            PlayerPrefs.SetString(_key, JsonConvert.SerializeObject(_history));
            PlayerPrefs.Save();
        }
    }
}