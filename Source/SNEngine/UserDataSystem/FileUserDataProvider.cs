using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SNEngine.Debugging;
using SNEngine.IO;
using SNEngine.UserDataSystem.Models;
using System;
using System.IO;

namespace SNEngine.UserDataSystem
{
    public class FileUserDataProvider : IUserDataProvider
    {
        private const string DATA_FOLDER_NAME = "UserData";
        private const string FILE_NAME = "userdata.json";

        private string GetFolderPath()
        {
            return Path.Combine(NovelDirectory.PersistentDataPath, DATA_FOLDER_NAME);
        }

        private string GetFilePath()
        {
            return Path.Combine(GetFolderPath(), FILE_NAME);
        }

        public async UniTask<UserData> LoadAsync()
        {
            string folderPath = GetFolderPath();
            string filePath = GetFilePath();

            try
            {
                if (!NovelDirectory.Exists(folderPath))
                {
                    NovelDirectory.Create(folderPath);
                    NovelGameDebug.Log($"[FileUserDataProvider] Created directory: {folderPath}");
                }

                if (!NovelFile.Exists(filePath))
                {
                    return new UserData();
                }

                string json = await NovelFile.ReadAllTextAsync(filePath);
                UserData data = JsonConvert.DeserializeObject<UserData>(json);
                return data;
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogWarning($"[FileUserDataProvider] Load failed: {ex.Message}");
                return new UserData();
            }
        }

        public UniTask SaveAsync(UserData data)
        {
            string folderPath = GetFolderPath();
            string filePath = GetFilePath();

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
                return NovelFile.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"[FileUserDataProvider] Save failed: {ex.Message}");
                return UniTask.CompletedTask;
            }
        }
    }
}