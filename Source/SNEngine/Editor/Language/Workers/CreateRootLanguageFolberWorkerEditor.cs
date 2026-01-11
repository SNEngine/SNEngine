using Cysharp.Threading.Tasks;
using SNEngine.Debugging;
using SNEngine.IO;
using System;
using System.IO;
using UnityEngine;

namespace SNEngine.Editor.Language.Workers
{
    [CreateAssetMenu(menuName = "SNEngine/Editor/Language/Workers/Create Root Language Folber Worker")]
    public class CreateRootLanguageFolderWorkerEditor : LanguageEditorWorker
    {
        public static string FolderName { get; set; }

        private const string ROOT_FOLDER = "Language";

        public override async UniTask<LanguageWorkerResult> Work()
        {
            LanguageWorkerResult result = new();

            if (string.IsNullOrEmpty(FolderName))
            {
                string error = $"[{nameof(CreateRootLanguageFolderWorkerEditor)}] Folder name not set";
                NovelGameDebug.LogError(error);
                result.Message = error;
                result.State = LanguageWorkerState.Error;
                return result;
            }

            try
            {
                string targetPath = Path.Combine(NovelDirectory.StreamingAssetsPath, ROOT_FOLDER, FolderName);

                if (!NovelDirectory.Exists(targetPath))
                {
                    await NovelDirectory.CreateAsync(targetPath);
                    NovelGameDebug.Log($"[{nameof(CreateRootLanguageFolderWorkerEditor)}] Folder created at: {targetPath}");
                }
                else
                {
                    NovelGameDebug.Log($"[{nameof(CreateRootLanguageFolderWorkerEditor)}] Folder already exists: {targetPath}");
                }

            }
            catch (Exception ex)
            {
                string error = $"[{nameof(CreateRootLanguageFolderWorkerEditor)}] Failed to create folder: {ex.Message}";
                NovelGameDebug.LogError(error);
                result.Message = error;
                result.State = LanguageWorkerState.Error;
            }

            return result;
        }
    }
}
