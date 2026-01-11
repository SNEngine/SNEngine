using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SNEngine.Debugging;
using SNEngine.UserDataSystem.Models;
using System;
using UnityEngine;

namespace SNEngine.UserDataSystem
{
    public class PlayerPrefsUserDataProvider : IUserDataProvider
    {
        private const string DATA_KEY = "SNEngine_UserData";

        public UniTask<UserData> LoadAsync()
        {
            string json = PlayerPrefs.GetString(DATA_KEY);

            if (string.IsNullOrEmpty(json))
            {
                NovelGameDebug.LogWarning($"[PlayerPrefsUserDataProvider] Key '{DATA_KEY}' not found in PlayerPrefs. Returning new UserData.");
                return UniTask.FromResult(new UserData());
            }

            try
            {
                UserData data = JsonConvert.DeserializeObject<UserData>(json);
                NovelGameDebug.Log($"[PlayerPrefsUserDataProvider] Loaded successfully from PlayerPrefs.");
                return UniTask.FromResult(data);
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"[PlayerPrefsUserDataProvider] Failed to deserialize data: {ex.Message}. Returning new UserData.");
                return UniTask.FromResult(new UserData());
            }
        }

        public UniTask SaveAsync(UserData data)
        {
            try
            {
                Formatting formatting =
#if UNITY_EDITOR
                    Formatting.Indented;
#else
                    Formatting.None;
#endif

                string json = JsonConvert.SerializeObject(data, formatting);
                PlayerPrefs.SetString(DATA_KEY, json);
                PlayerPrefs.Save();
                NovelGameDebug.Log($"[PlayerPrefsUserDataProvider] Saved to PlayerPrefs (Format: {formatting}).");
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"[PlayerPrefsUserDataProvider] Failed to save data: {ex.Message}");
            }

            return UniTask.CompletedTask;
        }
    }
}
