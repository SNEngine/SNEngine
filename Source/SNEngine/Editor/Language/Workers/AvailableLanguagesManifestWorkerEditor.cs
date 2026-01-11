using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SNEngine.Debugging;
using SNEngine.IO;
using SNEngine.Localization.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SNEngine.Editor.Language.Workers
{
    [CreateAssetMenu(menuName = "SNEngine/Editor/Language/Workers/Available Languages Manifest Worker")]
    public class AvailableLanguagesManifestWorkerEditor : LanguageEditorWorker
    {
        public static string PathSave { get; set; }
        private const string NAME_FILE = "language_manifest.json";

        public override async UniTask<LanguageWorkerResult> Work()
        {
            LanguageWorkerResult result = new();

            if (PathSave is null)
            {
                string error = $"[{nameof(AvailableLanguagesManifestWorkerEditor)}] Path save not set";
                NovelGameDebug.LogError(error);
                result.Message = error;
                result.State = LanguageWorkerState.Error;
                return result;
            }

            string languageRootPath = PathSave;
            string fullPath = Path.Combine(languageRootPath, NAME_FILE);

            if (!NovelDirectory.Exists(languageRootPath))
            {
                await NovelDirectory.CreateAsync(languageRootPath);
            }

            try
            {
                var languageFolders = NovelDirectory.GetDirectories(languageRootPath);

                var languages = languageFolders
                    .Select(dir => new LanguageEntry
                    {
                        Code = new DirectoryInfo(dir).Name
                    })
                    .ToList();

                var newManifest = new AvailableLanguagesManifest
                {
                    Languages = languages
                };

                string outputData = JsonConvert.SerializeObject(newManifest, Formatting.Indented);

                var utf8NoBom = new UTF8Encoding(false);
                await NovelFile.WriteAllTextAsync(fullPath, outputData, utf8NoBom);

                return result;
            }
            catch (Exception ex)
            {
                string error = $"[{nameof(AvailableLanguagesManifestWorkerEditor)}] Failed to generate manifest: {ex.Message}";
                NovelGameDebug.LogError(error);
                result.Message = error;
                result.State = LanguageWorkerState.Error;
                return result;
            }
        }
    }
}