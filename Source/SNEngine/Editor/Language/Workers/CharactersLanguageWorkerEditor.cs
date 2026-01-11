using Cysharp.Threading.Tasks;
using SharpYaml.Serialization;
using SNEngine.CharacterSystem;
using SNEngine.Debugging;
using SNEngine.IO;
using SNEngine.Localization.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace SNEngine.Editor.Language.Workers
{
    [CreateAssetMenu(menuName = "SNEngine/Editor/Language/Workers/Characters Language Worker")]
    public class CharactersLanguageWorkerEditor : LanguageEditorWorker
    {
        public static string PathSave { get; set; }
        private const string NAME_FILE = "characters.yaml";

        public override async UniTask<LanguageWorkerResult> Work()
        {
            LanguageWorkerResult result = new();

            if (string.IsNullOrEmpty(PathSave))
            {
                string error = $"[{nameof(CharactersLanguageWorkerEditor)}] PathSave not set";
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

            IEnumerable<Character> characters = Resources.LoadAll<Character>("Characters");
            List<CharacterLocalizationData> currentData = new();
            foreach (Character character in characters)
            {
                currentData.Add(new CharacterLocalizationData(character));
            }

            List<CharacterLocalizationData> existingData = null;

            if (File.Exists(fullPath))
            {
                try
                {
                    string existingYaml = await NovelFile.ReadAllTextAsync(fullPath);
                    Serializer deserializer = new Serializer();
                    existingData = deserializer.Deserialize<List<CharacterLocalizationData>>(existingYaml);
                }
                catch (Exception ex)
                {
                    string error = $"[{nameof(CharactersLanguageWorkerEditor)}] Failed to read existing file: {ex.Message}";
                    NovelGameDebug.LogError(error);
                    result.Message = error;
                    result.State = LanguageWorkerState.Error;
                }
            }

            List<CharacterLocalizationData> mergedData = MergeCharacterData(existingData, currentData);

            Serializer serializer = new Serializer();
            string outputData = serializer.Serialize(mergedData);
            var utf8NoBom = new UTF8Encoding(false);
            await NovelFile.WriteAllTextAsync(fullPath, outputData, utf8NoBom);

            return result;
        }

        private List<CharacterLocalizationData> MergeCharacterData(
            List<CharacterLocalizationData> existing,
            List<CharacterLocalizationData> current)
        {
            if (existing == null)
                return current;

            var existingDict = new Dictionary<string, CharacterLocalizationData>();
            foreach (var c in existing)
                existingDict[c.GUID] = c;

            List<CharacterLocalizationData> result = new();
            var stringFields = typeof(CharacterLocalizationData)
                .GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var c in current)
            {
                if (existingDict.TryGetValue(c.GUID, out var old))
                {
                    var merged = new CharacterLocalizationData();
                    foreach (var field in stringFields)
                    {
                        if (field.FieldType != typeof(string))
                            continue;

                        string currentValue = (string)field.GetValue(c);
                        string oldValue = (string)field.GetValue(old);

                        field.SetValue(merged, oldValue != currentValue ? oldValue : currentValue);
                    }

                    merged.GUID = c.GUID;
                    result.Add(merged);
                }
                else
                {
                    result.Add(c);
                }
            }

            return result;
        }

    }
}
