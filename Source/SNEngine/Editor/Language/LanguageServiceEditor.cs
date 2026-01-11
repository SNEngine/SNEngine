using Cysharp.Threading.Tasks;
using SNEngine.Debugging;
using SNEngine.Editor.Language.Workers;
using SNEngine.IO;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor.Language
{
    [CreateAssetMenu(menuName = "SNEngine/Editor/Language/Editor Service")]
    public class LanguageServiceEditor : ScriptableObject
    {
        [SerializeField] private LanguageEditorWorker[] _workers;

        public IEnumerable<LanguageEditorWorker> Workers => _workers;

        public async UniTask RunAllWorkersAsync()
        {
            if (_workers == null || _workers.Length == 0)
            {
               NovelGameDebug.LogWarning($"[{nameof(LanguageServiceEditor)}] No workers to run.");
                return;
            }

            string title = "Language Service…";
            int total = _workers.Length;

            for (int i = 0; i < total; i++)
            {
                var worker = _workers[i];
                string info = $"Executing worker {i + 1}/{total}: {worker.GetType().Name}";

                EditorUtility.DisplayProgressBar(title, info, (float)i / (float)total);

                try
                {
                    var result = await worker.Work();

                    if (result.State == LanguageWorkerState.Error)
                    {
                       NovelGameDebug.LogError($"[{nameof(LanguageServiceEditor)}] Worker {worker.GetType().Name} failed: {result.Message}");
                        break;
                    }
                }
                catch (Exception ex)
                {
                   NovelGameDebug.LogError($"[{nameof(LanguageServiceEditor)}] Worker {worker.GetType().Name} threw exception: {ex}");
                    break;
                }
            }

            EditorUtility.DisplayProgressBar(title, "Finishing…", 1f);
            EditorUtility.ClearProgressBar();

           NovelGameDebug.Log($"[{nameof(LanguageServiceEditor)}] All workers completed.");
           AssetDatabase.Refresh();
        }

        public IEnumerable<string> GetAvailableLanguages()
        {
            string langsPath = Path.Combine(Application.dataPath, "StreamingAssets/Language");
            if (!Directory.Exists(langsPath)) return new List<string>();

            var dirs = NovelDirectory.GetDirectories(langsPath);
            var languages = new List<string>();
            foreach (var dir in dirs)
                languages.Add(Path.GetFileName(dir));
            return languages;
        }

        public string GetLanguagePath(string codeLanguage)
        {
            return Path.Combine(Application.dataPath, "StreamingAssets/Language", codeLanguage);
        }
    }
}
