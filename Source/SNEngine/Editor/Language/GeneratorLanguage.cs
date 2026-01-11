using Cysharp.Threading.Tasks;
using SNEngine.Debugging;
using SNEngine.Editor.Language.Workers;
using SNEngine.IO;
using System;
using System.IO;
using UnityEngine;

namespace SNEngine.Editor.Language
{
    public static class GeneratorLanguage
    {
        public static async UniTask Generate(string nameLanguage, string flagPath = "")
        {
            if (string.IsNullOrEmpty(nameLanguage))
            {
                NovelGameDebug.LogError("[GeneratorLanguage] nameLanguage not set");
                return;
            }

            LanguageServiceEditor languageService = Resources.Load<LanguageServiceEditor>("Editor/SO/Language Service Editor");
            if (languageService == null)
            {
                NovelGameDebug.LogError("[GeneratorLanguage] Failed to load LanguageServiceEditor");
                return;
            }

            string mainPath = Path.Combine(NovelDirectory.StreamingAssetsPath, "Language", nameLanguage);

            CreateRootLanguageFolderWorkerEditor.FolderName = nameLanguage;
            MetaDataLanguageWorkerEditor.PathSave = mainPath;
            MetaDataLanguageWorkerEditor.MetaData = new()
            {
                NameLanguage = nameLanguage,
                Version = 1,
                Author = Environment.UserName,
            };
            CharactersLanguageWorkerEditor.PathSave = mainPath;
            DialoguesLanguageWorkerEditor.PathSave = mainPath;
            FlagsLanguageWorkerEditor.FlagToUse = flagPath;
            FlagsLanguageWorkerEditor.PathSave = mainPath;
            UITemplateLanguageWorkerEditor.PathSave = mainPath;
            LanguageManifestWorkerEditor.PathSave = mainPath;
            AvailableLanguagesManifestWorkerEditor.PathSave = Path.Combine(NovelDirectory.StreamingAssetsPath, "Language");
            await languageService.RunAllWorkersAsync();
        }
    }
}
