using Cysharp.Threading.Tasks;
using SharpYaml.Serialization;
using SNEngine.CharacterSystem;
using SNEngine.Debugging;
using SNEngine.Graphs;
using SNEngine.IO;
using SNEngine.Localization;
using SNEngine.Localization.Models;
using SNEngine.SaveSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Language Service")]
    public class LanguageService : ServiceBase
    {
        private Dictionary<string, CharacterLocalizationData> _chatacterLocalizeData = new Dictionary<string, CharacterLocalizationData>();
        private Dictionary<string, NodeLocalizationData> _nodesLocalizeData = new Dictionary<string, NodeLocalizationData>();
        private Dictionary<string, string> _uiLocalizeData = new Dictionary<string, string>();
        private LanguageMetaData _metaData;
        private Texture2D _flag;
        private Dictionary<string, object> _originalNodeValues;
        private DialogueGraph _currentGraph;
        public event Action<string> OnLanguageLoaded;

        private const string LanguageBaseDir = "Language";

#if UNITY_EDITOR
        [SerializeField] private string _testLang = "ru";
#endif

        public bool LanguageIsLoaded => _metaData != null;
        public string CurrentLanguageCode { get; private set; } = "None";
        public LanguageMetaData MetaData => _metaData;
        public Texture2D Flag => _flag;

        public override void Initialize()
        {
            base.Initialize();
            LoadDefaultLanguage().Forget();
        }

        private async UniTask LoadDefaultLanguage()
        {
            UserDataService userDataService = NovelGame.Instance.GetService<UserDataService>();
            string defaultLangCode = "en";
            string langToLoad = defaultLangCode;

            if (string.IsNullOrEmpty(userDataService.Data.CurrentLanguage))
            {
                NovelGameDebug.Log($"[{nameof(LanguageService)}] UserData CurrentLanguage is empty. Setting to default: {defaultLangCode}");
                await LoadLanguage(langToLoad);

                if (LanguageIsLoaded)
                {
                    userDataService.Data.CurrentLanguage = langToLoad;
                    await userDataService.SaveAsync();
                    NovelGameDebug.Log($"[{nameof(LanguageService)}] Current language seted to {defaultLangCode}");
                }
            }

        }


#if UNITY_EDITOR
        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode || state == PlayModeStateChange.ExitingEditMode)
                RestoreOriginalNodeValuesOnExit();
        }

        private void RestoreOriginalNodeValuesOnExit()
        {
            if (_originalNodeValues == null || _currentGraph == null) return;
            foreach (var node in _currentGraph.AllNodes.Select(x => x.Value).OfType<ILocalizationNode>())
            {
                if (_originalNodeValues.TryGetValue(node.GUID, out var value))
                    node.SetValue(value);
            }
            _originalNodeValues.Clear();
            _currentGraph = null;
        }
#endif

        public async UniTask LoadLanguage(string codeLanguage)
        {
            CurrentLanguageCode = "None";

            string basePath = Path.Combine(NovelDirectory.StreamingAssetsPath, LanguageBaseDir);

            if (!Directory.Exists(basePath))
            {
                NovelGameDebug.Log($"[{nameof(LanguageService)}] Localization directory not found in StreamingAssets. Please read the documentation: https://github.com/SNEngine/SNEngineDocs/wiki/Localization");
                return;
            }

            string langFolder = Path.Combine(basePath, codeLanguage);

            LanguageManifest manifest = await LoadManifestAsync(langFolder);
            if (manifest == null)
            {
                return;
            }

            await LoadCharactersAsync(langFolder, manifest);
            await LoadFlagAsync(langFolder, manifest);
            await LoadMetadataAsync(langFolder, manifest);
            await LoadDialoguesAsync(langFolder, manifest);
            await LoadUIAsync(langFolder, manifest);

            CurrentLanguageCode = codeLanguage;
            OnLanguageLoaded?.Invoke(codeLanguage);
        }

        private async UniTask<LanguageManifest> LoadManifestAsync(string langFolder)
        {
            string path = Path.Combine(langFolder, "manifest.json");
            NovelGameDebug.Log($"[{nameof(LanguageService)}] Loading manifest from: {path}");

            try
            {
                string json = await NovelFile.ReadAllTextAsync(path);
                return JsonConvert.DeserializeObject<LanguageManifest>(json);
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"manifest.json not found or failed to read: {path}. Error: {ex.Message}");
                return null;
            }
        }

        private async UniTask LoadCharactersAsync(string langFolder, LanguageManifest manifest)
        {
            _chatacterLocalizeData = new Dictionary<string, CharacterLocalizationData>();
            if (string.IsNullOrEmpty(manifest.Characters))
            {
                NovelGameDebug.Log($"[{nameof(LanguageService)}] Characters path is empty, skipping character loading.");
                return;
            }
            NovelGameDebug.Log($"[{nameof(LanguageService)}] Loading characters from: {Path.Combine(langFolder, manifest.Characters)}");

            try
            {
                string yaml = await NovelFile.ReadAllTextAsync(Path.Combine(langFolder, manifest.Characters));
                Serializer deserializer = new Serializer();
                foreach (var c in deserializer.Deserialize<List<CharacterLocalizationData>>(yaml))
                    _chatacterLocalizeData[c.GUID] = c;
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"Failed to load characters.yaml: {ex.Message}");
            }
        }

        private async UniTask LoadFlagAsync(string langFolder, LanguageManifest manifest)
        {
            _flag = null;
            if (string.IsNullOrEmpty(manifest.Flag))
            {
                NovelGameDebug.Log($"[{nameof(LanguageService)}] Flag path is empty, skipping flag loading.");
                return;
            }
            NovelGameDebug.Log($"[{nameof(LanguageService)}] Loading flag from: {Path.Combine(langFolder, manifest.Flag)}");

            try
            {
                byte[] bytes = await NovelFile.ReadAllBytesAsync(Path.Combine(langFolder, manifest.Flag));
                _flag = new Texture2D(2, 2);
                _flag.LoadImage(bytes);
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"Failed to load flag.png: {ex.Message}");
            }
        }

        private async UniTask LoadMetadataAsync(string langFolder, LanguageManifest manifest)
        {
            _metaData = null;
            if (string.IsNullOrEmpty(manifest.Metadata))
            {
                NovelGameDebug.Log($"[{nameof(LanguageService)}] Metadata path is empty, skipping metadata loading.");
                return;
            }
            NovelGameDebug.Log($"[{nameof(LanguageService)}] Loading metadata from: {Path.Combine(langFolder, manifest.Metadata)}");

            try
            {
                string yaml = await NovelFile.ReadAllTextAsync(Path.Combine(langFolder, manifest.Metadata));
                Serializer deserializer = new Serializer();
                _metaData = deserializer.Deserialize<LanguageMetaData>(yaml);
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"Failed to load metadata.yaml: {ex.Message}");
            }
        }

        private async UniTask LoadDialoguesAsync(string langFolder, LanguageManifest manifest)
        {
            _nodesLocalizeData = new Dictionary<string, NodeLocalizationData>();
            if (manifest.Dialogues == null)
            {
                NovelGameDebug.Log($"[{nameof(LanguageService)}] Dialogues list is null, skipping dialogue loading.");
                return;
            }
            NovelGameDebug.Log($"[{nameof(LanguageService)}] Loading {manifest.Dialogues.Count} dialogue files.");

            Serializer deserializer = new Serializer();
            foreach (var file in manifest.Dialogues)
            {
                try
                {
                    string yaml = await NovelFile.ReadAllTextAsync(Path.Combine(langFolder, file));
                    var dict = deserializer.Deserialize<Dictionary<string, object>>(yaml);
                    foreach (var kvp in dict)
                        _nodesLocalizeData[kvp.Key] = new NodeLocalizationData { GUID = kvp.Key, Value = kvp.Value };
                }
                catch (Exception ex)
                {
                    NovelGameDebug.LogError($"Failed to load dialogue {file}: {ex.Message}");
                }
            }
        }

        private async UniTask LoadUIAsync(string langFolder, LanguageManifest manifest)
        {
            _uiLocalizeData = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(manifest.Ui))
            {
                NovelGameDebug.Log($"[{nameof(LanguageService)}] UI path is empty, skipping UI loading.");
                return;
            }
            NovelGameDebug.Log($"[{nameof(LanguageService)}] Loading UI localization from: {Path.Combine(langFolder, manifest.Ui)}");

            try
            {
                string yaml = await NovelFile.ReadAllTextAsync(Path.Combine(langFolder, manifest.Ui));
                Serializer deserializer = new Serializer();
                _uiLocalizeData = deserializer.Deserialize<Dictionary<string, string>>(yaml) ?? new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"Failed to load ui.yaml: {ex.Message}");
            }
        }

        public async UniTask<List<LanguageEntry>> GetAvailableLanguagesAsync()
        {
            string path = Path.Combine(NovelDirectory.StreamingAssetsPath, LanguageBaseDir, "language_manifest.json");

            if (!File.Exists(path))
            {
                NovelGameDebug.Log($"[{nameof(LanguageService)}] Localization manifest not found. Please read the documentation: https://github.com/SNEngine/SNEngineDocs/wiki/Localization");
                return new List<LanguageEntry>();
            }

            try
            {
                string json = await NovelFile.ReadAllTextAsync(path);
                var manifest = JsonConvert.DeserializeObject<AvailableLanguagesManifest>(json);
                return manifest?.Languages ?? new List<LanguageEntry>();
            }
            catch (Exception)
            {
                return new List<LanguageEntry>();
            }
        }
        public async UniTask<Dictionary<string, PreloadLanguageData>> LoadAvailableLanguagesPreloadDataAsync()
        {
            NovelGameDebug.Log($"[{nameof(LanguageService)}] Starting preload data loading for available languages.");
            var availableLanguages = new Dictionary<string, PreloadLanguageData>();
            var languageEntries = await GetAvailableLanguagesAsync();
            Serializer deserializer = new Serializer();

            foreach (var entry in languageEntries)
            {
                string codeLanguage = entry.Code;
                string langFolder = Path.Combine(NovelDirectory.StreamingAssetsPath, LanguageBaseDir, codeLanguage);
                NovelGameDebug.Log($"[{nameof(LanguageService)}] Processing language: {codeLanguage}");

                LanguageManifest manifest = await LoadManifestAsync(langFolder);
                if (manifest == null) continue;

                LanguageMetaData metaData = null;
                if (!string.IsNullOrEmpty(manifest.Metadata))
                {
                    try
                    {
                        string yaml = await NovelFile.ReadAllTextAsync(Path.Combine(langFolder, manifest.Metadata));
                        metaData = deserializer.Deserialize<LanguageMetaData>(yaml);
                    }
                    catch (Exception ex)
                    {
                        NovelGameDebug.LogError($"Failed to load metadata for language {codeLanguage}: {ex.Message}");
                    }
                }

                if (metaData != null)
                {
                    string flagPath = null;
                    if (!string.IsNullOrEmpty(manifest.Flag))
                    {
                        flagPath = NovelFile.GetAbsolutePath(Path.Combine(langFolder, manifest.Flag));
                    }

                    availableLanguages.Add(codeLanguage, new PreloadLanguageData
                    {
                        CodeLanguage = codeLanguage,
                        MetaData = metaData,
                        PathFlag = flagPath
                    });
                }
            }
            NovelGameDebug.Log($"[{nameof(LanguageService)}] Finished preload data loading. Found {availableLanguages.Count} languages.");
            return availableLanguages;
        }


        public void TransliteGraph(DialogueGraph graph)
        {
            if (!LanguageIsLoaded) return;
            _currentGraph = graph;
            _originalNodeValues = new Dictionary<string, object>();

            foreach (var node in _currentGraph.AllNodes.Select(x => x.Value).OfType<ILocalizationNode>())
            {
                _originalNodeValues[node.GUID] = node.GetValue();
                if (_nodesLocalizeData.TryGetValue(node.GUID, out var localize))
                    node.SetValue(localize.Value);
            }

#if UNITY_EDITOR
            graph.OnEndExecute -= RestoreOriginalValues;
            graph.OnEndExecute += RestoreOriginalValues;
#endif
        }

        public string TransliteNameCharacter(Character character)
        {
            if (!LanguageIsLoaded) return character.OriginalName;
            return _chatacterLocalizeData.TryGetValue(character.GUID, out var loc) ? loc.Name : character.OriginalName;
        }

        public string TransliteUI(string key)
        {
            if (!LanguageIsLoaded) return key;
            return _uiLocalizeData.TryGetValue(key, out var value) ? value : key;
        }

#if UNITY_EDITOR
        private void RestoreOriginalValues()
        {
            if (_originalNodeValues == null) return;
            foreach (var node in _currentGraph.AllNodes.Select(x => x.Value).OfType<ILocalizationNode>())
            {
                if (_originalNodeValues.TryGetValue(node.GUID, out var value))
                    node.SetValue(value);
            }
        }
#endif
    }
}