using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SNEngine.Debugging;
using SNEngine.IO;
using SNEngine.Localization.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SNEngine.Editor.Language.Workers
{
    [CreateAssetMenu(menuName = "SNEngine/Editor/Language/Workers/Language Manifest Worker")]
    public class LanguageManifestWorkerEditor : LanguageEditorWorker
    {
        public static string PathSave { get; set; }

        private const string CharactersFileName = "characters.yaml";
        private const string UIFileName = "ui.yaml";
        private const string MetaDataFileName = "metadata.yaml";
        private const string FlagFileName = "flag.png";
        private const string DialogueSubDir = "dialogues";

        private const string NAME_FILE = "manifest.json";

        public override async UniTask<LanguageWorkerResult> Work()
        {
            LanguageWorkerResult result = new();

            if (PathSave is null)
            {
                string error = $"[{nameof(LanguageManifestWorkerEditor)}] Path save not set";
                NovelGameDebug.LogError(error);
                result.Message = error;
                result.State = LanguageWorkerState.Error;
                return result;
            }

            string fullManifestPath = Path.Combine(PathSave, NAME_FILE);

            if (!NovelDirectory.Exists(PathSave))
            {
                await NovelDirectory.CreateAsync(PathSave);
            }

            var dialogueFullPath = Path.Combine(PathSave, DialogueSubDir);
            List<string> dialogueRelativePaths = new List<string>();

            if (NovelDirectory.Exists(dialogueFullPath))
            {
                try
                {
                    var dialogueFiles = NovelDirectory.GetFiles(dialogueFullPath, "*.yaml", SearchOption.AllDirectories);

                    int stripLength = PathSave.Length + 1;
                    dialogueRelativePaths = dialogueFiles
                        .Select(fullPath => fullPath.Substring(stripLength).Replace('\\', '/'))
                        .ToList();
                }
                catch (Exception ex)
                {
                    string error = $"[{nameof(LanguageManifestWorkerEditor)}] Failed to scan dialogue directory: {ex.Message}";
                    NovelGameDebug.LogError(error);
                    result.Message = error;
                    result.State = LanguageWorkerState.Error;
                    return result;
                }
            }

            var newManifest = new LanguageManifest
            {
                Characters = NovelFile.Exists(Path.Combine(PathSave, CharactersFileName)) ? CharactersFileName : null,
                Ui = NovelFile.Exists(Path.Combine(PathSave, UIFileName)) ? UIFileName : null,
                Metadata = NovelFile.Exists(Path.Combine(PathSave, MetaDataFileName)) ? MetaDataFileName : null,
                Flag = NovelFile.Exists(Path.Combine(PathSave, FlagFileName)) ? FlagFileName : null,

                Dialogues = dialogueRelativePaths.Any() ? dialogueRelativePaths : null
            };

            string outputData = JsonConvert.SerializeObject(newManifest, Formatting.Indented);

            var utf8NoBom = new UTF8Encoding(false);
            await NovelFile.WriteAllTextAsync(fullManifestPath, outputData, utf8NoBom);

            return result;
        }
    }
}