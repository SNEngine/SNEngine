using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SNEngine.Debugging;
using SNEngine.SaveSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SNEngine.SaveSystem
{
    public class PlayerPrefsSaveLoadProvider : ISaveLoadProvider
    {
        private const string BASE_KEY = "SNEngine_Saves_";
        private const string SAVE_KEY_FORMAT = BASE_KEY + "{0}_data";
        private const string PREVIEW_KEY_FORMAT = BASE_KEY + "{0}_preview";
        private const string LIST_KEY = BASE_KEY + "List";

        private List<string> _availableSaves = new List<string>();

        public PlayerPrefsSaveLoadProvider()
        {
            string jsonList = PlayerPrefs.GetString(LIST_KEY, "[]");
            try
            {
                _availableSaves = JsonConvert.DeserializeObject<List<string>>(jsonList);
            }
            catch
            {
                _availableSaves = new List<string>();
            }
        }

        public UniTask SaveAsync(string saveName, SaveData data, Texture2D previewTexture)
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
                PlayerPrefs.SetString(string.Format(SAVE_KEY_FORMAT, saveName), json);

                if (previewTexture != null)
                {
                    byte[] bytes = previewTexture.EncodeToPNG();
                    string base64Preview = Convert.ToBase64String(bytes);
                    PlayerPrefs.SetString(string.Format(PREVIEW_KEY_FORMAT, saveName), base64Preview);
                }

                if (!_availableSaves.Contains(saveName))
                {
                    _availableSaves.Add(saveName);
                    string listJson = JsonConvert.SerializeObject(_availableSaves);
                    PlayerPrefs.SetString(LIST_KEY, listJson);
                }

                PlayerPrefs.Save();
                NovelGameDebug.Log($"[PlayerPrefsSaveLoadProvider] Saved data for: {saveName}");
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"[PlayerPrefsSaveLoadProvider] Failed to save '{saveName}': {ex.Message}");
            }

            return UniTask.CompletedTask;
        }

        public async UniTask<PreloadSave> LoadPreloadSaveAsync(string saveName)
        {
            try
            {
                string json = PlayerPrefs.GetString(string.Format(SAVE_KEY_FORMAT, saveName));
                if (string.IsNullOrEmpty(json))
                {
                    NovelGameDebug.LogError($"[PlayerPrefsSaveLoadProvider] Save file not found for: {saveName}");
                    return null;
                }

                SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
                Texture2D previewTexture = await LoadTextureAsync(saveName);

                NovelGameDebug.Log($"[PlayerPrefsSaveLoadProvider] Loaded preload save data: {saveName}");

                return new PreloadSave
                {
                    SaveData = saveData,
                    PreviewTexture = previewTexture,
                    SaveName = saveName
                };
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"[PlayerPrefsSaveLoadProvider] Failed to load/deserialize '{saveName}': {ex.Message}");
                return null;
            }
        }

        public async UniTask<SaveData> LoadSaveAsync(string saveName)
        {
            try
            {
                string json = PlayerPrefs.GetString(string.Format(SAVE_KEY_FORMAT, saveName));
                if (string.IsNullOrEmpty(json))
                {
                    NovelGameDebug.LogError($"[PlayerPrefsSaveLoadProvider] Save file not found for: {saveName}");
                    return null;
                }

                SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
                NovelGameDebug.Log($"[PlayerPrefsSaveLoadProvider] Loaded save: {saveName}");
                return saveData;
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"[PlayerPrefsSaveLoadProvider] Failed to load/deserialize '{saveName}': {ex.Message}");
                return null;
            }
        }

        public UniTask<IEnumerable<string>> GetAllAvailableSavesAsync()
        {
            return UniTask.FromResult(_availableSaves.AsEnumerable());
        }

        private UniTask<Texture2D> LoadTextureAsync(string saveName)
        {
            string base64Preview = PlayerPrefs.GetString(string.Format(PREVIEW_KEY_FORMAT, saveName));

            if (string.IsNullOrEmpty(base64Preview))
            {
                NovelGameDebug.LogWarning($"[PlayerPrefsSaveLoadProvider] Preview data not found for: {saveName}");
                return UniTask.FromResult<Texture2D>(null);
            }

            try
            {
                byte[] bytes = Convert.FromBase64String(base64Preview);

                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);

                return UniTask.FromResult(texture);
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"[PlayerPrefsSaveLoadProvider] Failed to load/decode preview for '{saveName}': {ex.Message}");
                return UniTask.FromResult<Texture2D>(null);
            }
        }

        public async UniTask<bool> DeleteSaveAsync(string saveName)
        {
            try
            {
                PlayerPrefs.DeleteKey(string.Format(SAVE_KEY_FORMAT, saveName));
                PlayerPrefs.DeleteKey(string.Format(PREVIEW_KEY_FORMAT, saveName));

                if (_availableSaves.Contains(saveName))
                {
                    _availableSaves.Remove(saveName);
                    string listJson = JsonConvert.SerializeObject(_availableSaves);
                    PlayerPrefs.SetString(LIST_KEY, listJson);
                }

                PlayerPrefs.Save();
                NovelGameDebug.Log($"[PlayerPrefsSaveLoadProvider] Deleted save: {saveName}");
                return true;
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"[PlayerPrefsSaveLoadProvider] Failed to delete save '{saveName}': {ex.Message}");
                return false;
            }
        }
    }
}