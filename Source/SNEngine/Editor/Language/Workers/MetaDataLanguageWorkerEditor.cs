using Cysharp.Threading.Tasks;
using SharpYaml.Serialization;
using SNEngine.Debugging;
using SNEngine.IO;
using SNEngine.Localization.Models;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace SNEngine.Editor.Language.Workers
{
    [CreateAssetMenu(menuName = "SNEngine/Editor/Language/Workers/MetaData Worker")]
    public class MetaDataLanguageWorkerEditor : LanguageEditorWorker
    {
        public static string PathSave { get; set; }
        public static LanguageMetaData MetaData { get; set; }
        private const string NAME_FILE = "metadata.yaml";

        public override async UniTask<LanguageWorkerResult> Work()
        {
            LanguageWorkerResult result = new();

            if (MetaData is null)
            {
                string error = $"[{nameof(MetaDataLanguageWorkerEditor)}] Meta data not set";
                NovelGameDebug.LogError(error);
                result.Message = error;
                result.State = LanguageWorkerState.Error;
                return result;
            }

            if (PathSave is null)
            {
                string error = $"[{nameof(MetaDataLanguageWorkerEditor)}] Path save not set";
                NovelGameDebug.LogError(error);
                result.Message = error;
                result.State = LanguageWorkerState.Error;
                return result;
            }

            string fullPath = Path.Combine(PathSave, NAME_FILE);
            string directory = Path.GetDirectoryName(fullPath);

            if (!NovelDirectory.Exists(directory))
            {
                await NovelDirectory.CreateAsync(directory);
            }

            LanguageMetaData existingData = null;

            if (File.Exists(fullPath))
            {
                try
                {
                    string existingYaml = await NovelFile.ReadAllTextAsync(fullPath);
                    Serializer deserializer = new Serializer();
                    existingData = deserializer.Deserialize<LanguageMetaData>(existingYaml);
                }
                catch (Exception ex)
                {
                    string error = $"[{nameof(MetaDataLanguageWorkerEditor)}] Failed to read existing file: {ex.Message}";
                    NovelGameDebug.LogError(error);
                    result.Message = error;
                    result.State = LanguageWorkerState.Error;
                }
            }

            var mergedData = MergeMetaData(existingData, MetaData);

            Serializer serializer = new Serializer();
            string outputData = serializer.Serialize(mergedData);
            var utf8NoBom = new UTF8Encoding(false);
            await NovelFile.WriteAllTextAsync(fullPath, outputData, utf8NoBom);

            return result;
        }

        private LanguageMetaData MergeMetaData(LanguageMetaData existing, LanguageMetaData current)
        {
            if (existing is null)
            {
                return current;
            }

            var result = new LanguageMetaData
            {
                NameLanguage = string.IsNullOrEmpty(existing.NameLanguage) ? current.NameLanguage : existing.NameLanguage,
            };

            return result;
        }
    }
}
