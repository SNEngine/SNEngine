using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SNEngine.Debugging;
using SNEngine.IO;
using SNEngine.SaveSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SNEngine.SaveSystem
{
    public class FileSaveLoadProvider : ISaveLoadProvider
    {
        private const string SAVE_FOLDER_NAME = "saves";
        private const string SAVE_FILE_NAME = "progress.json";
        private const string PREVIEW_FILE_NAME = "preview.png";

        public UniTask SaveAsync(string saveName, SaveData data, Texture2D previewTexture)
        {
            string folderPath = GetSaveFolderPath(saveName);
            string saveFilePath = GetSaveFilePath(saveName);
            string previewFilePath = GetPreviewFilePath(saveName);

            try
            {
                if (!NovelDirectory.Exists(folderPath))
                {
                    NovelDirectory.Create(folderPath);
                }

                Formatting formatting =
#if UNITY_EDITOR
                    Formatting.Indented;
#else
                    Formatting.None;
#endif

                string json = JsonConvert.SerializeObject(data, formatting);

                NovelGameDebug.Log($"[FileSaveLoadProvider] Saving data to: {saveFilePath}");

                UniTask saveJsonTask = NovelFile.WriteAllTextAsync(saveFilePath, json);

                UniTask savePreviewTask = SaveTextureToPNGAsync(previewTexture, previewFilePath);

                return UniTask.WhenAll(saveJsonTask, savePreviewTask);
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"[FileSaveLoadProvider] Failed to save '{saveName}': {ex.Message}");
                return UniTask.CompletedTask;
            }
        }

        public async UniTask<PreloadSave> LoadPreloadSaveAsync(string saveName)
        {
            string saveFilePath = GetSaveFilePath(saveName);
            string previewFilePath = GetPreviewFilePath(saveName);

            try
            {
                string json = await NovelFile.ReadAllTextAsync(saveFilePath);
                SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);

                Texture2D previewTexture = await LoadTextureAsync(previewFilePath);

                NovelGameDebug.Log($"[FileSaveLoadProvider] Loaded preload save data: {saveName} from {saveFilePath}");

                return new PreloadSave
                {
                    SaveData = saveData,
                    PreviewTexture = previewTexture,
                    SaveName = saveName
                };
            }
            catch (FileNotFoundException)
            {
                NovelGameDebug.LogError($"[FileSaveLoadProvider] Save file not found for: {saveName}");
                return null;
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"[FileSaveLoadProvider] Failed to load/deserialize '{saveName}': {ex.Message}");
                return null;
            }
        }

        public async UniTask<SaveData> LoadSaveAsync(string saveName)
        {
            string saveFilePath = GetSaveFilePath(saveName);

            try
            {
                string json = await NovelFile.ReadAllTextAsync(saveFilePath);
                SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
                NovelGameDebug.Log($"[FileSaveLoadProvider] Loaded save: {saveName} from {saveFilePath}");
                return saveData;


            }
            catch (FileNotFoundException)
            {
                NovelGameDebug.LogError($"[FileSaveLoadProvider] Save file not found for: {saveName}");
                return null;
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"[FileSaveLoadProvider] Failed to load/deserialize '{saveName}': {ex.Message}");
                return null;
            }
        }

        public UniTask<IEnumerable<string>> GetAllAvailableSavesAsync()
        {
            string rootPath = GetRootSaveFolderPath();

            if (!NovelDirectory.Exists(rootPath))
            {
                return UniTask.FromResult(Enumerable.Empty<string>());
            }

            string[] saveFolders = Directory.GetDirectories(rootPath);

            IEnumerable<string> saveNames = saveFolders.Select(Path.GetFileName);

            return UniTask.FromResult(saveNames);
        }

        private UniTask SaveTextureToPNGAsync(Texture2D texture, string path)
        {
            if (texture == null) return UniTask.CompletedTask;

            byte[] bytes = texture.EncodeToPNG();

            return NovelFile.WriteAllBytesAsync(path, bytes);
        }

        private async UniTask<Texture2D> LoadTextureAsync(string path)
        {
            if (!NovelFile.Exists(path))
            {
                NovelGameDebug.LogWarning($"[FileSaveLoadProvider] Preview file not found at: {path}");
                return null;
            }

            byte[] bytes = await NovelFile.ReadAllBytesAsync(path);

            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);

            return texture;
        }

        private string GetRootSaveFolderPath()
        {
            return Path.Combine(NovelDirectory.PersistentDataPath, SAVE_FOLDER_NAME);
        }

        private string GetSaveFolderPath(string saveName)
        {
            return Path.Combine(GetRootSaveFolderPath(), saveName);
        }

        private string GetSaveFilePath(string saveName)
        {
            return Path.Combine(GetSaveFolderPath(saveName), SAVE_FILE_NAME);
        }

        private string GetPreviewFilePath(string saveName)
        {
            return Path.Combine(GetSaveFolderPath(saveName), PREVIEW_FILE_NAME);
        }

        public async UniTask<bool> DeleteSaveAsync(string saveName)
        {
            string saveFolderPath = GetSaveFolderPath(saveName);

            try
            {
                if (NovelDirectory.Exists(saveFolderPath))
                {
                    NovelDirectory.Delete(saveFolderPath, true);
                    NovelGameDebug.Log($"[FileSaveLoadProvider] Deleted save: {saveName} at {saveFolderPath}");
                    return true;
                }
                else
                {
                    NovelGameDebug.LogWarning($"[FileSaveLoadProvider] Save folder not found for deletion: {saveName}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"[FileSaveLoadProvider] Failed to delete save '{saveName}': {ex.Message}");
                return false;
            }
        }
    }
}