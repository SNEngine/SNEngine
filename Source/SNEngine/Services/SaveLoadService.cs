using Cysharp.Threading.Tasks;
using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.Debugging;
using SNEngine.Graphs;
using SNEngine.SaveSystem.Models;
using SNEngine.Services;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using SaveData = SNEngine.SaveSystem.Models.SaveData;

namespace SNEngine.SaveSystem
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Save Load Service")]
    public class SaveLoadService : ServiceBase, IService
    {
        private const int PREVIEW_IMAGE_SIZE = 1512;
        private const int WEBGL_PREVIEW_SIZE = 256;

        private ISaveLoadProvider _provider;
        private Dictionary<string, object> _originalVariableValues;
        private DialogueGraph _currentGraph;

        public override void Initialize()
        {
            base.Initialize();

#if UNITY_WEBGL
            _provider = new PlayerPrefsSaveLoadProvider();
            NovelGameDebug.Log("[SaveLoadService] Initialized with PlayerPrefsSaveLoadProvider for WebGL.");
#else
            _provider = new FileSaveLoadProvider();
            NovelGameDebug.Log("[SaveLoadService] Initialized with FileSaveLoadProvider for FileSystem.");
#endif
        }

        public SaveData CaptureCurrentState()
        {
            try
            {
                var dialogueService = NovelGame.Instance.GetService<DialogueService>();
                var globalVariablesService = NovelGame.Instance.GetService<VariablesContainerService>();

                if (dialogueService.CurrentDialogue is not DialogueGraph dialogueGraph)
                {
                    return null;
                }

                var nodeGuid = dialogueGraph.CurrentExecuteNode.GUID;
                var variables = dialogueGraph.Variables;
                var globalVariables = globalVariablesService.GlobalVariables;

                Dictionary<string, object> variablesData = new();
                foreach (var variable in variables)
                {
                    variablesData.Add(variable.Value.GUID, variable.Value.GetCurrentValue());
                }

                Dictionary<string, object> globalVariablesData = new();
                foreach (var variable in globalVariables)
                {
                    globalVariablesData.Add(variable.Value.GUID, variable.Value.GetCurrentValue());
                }

                return new SaveData
                {
                    CurrentNode = nodeGuid,
                    Variables = variablesData,
                    GlobalVariables = globalVariablesData,
                    DialogueGUID = dialogueGraph.GUID,
                    DateSave = System.DateTime.Now,
                    NodesData = ExtractSaveDataFromGraph(dialogueGraph),
                };
            }
            catch
            {
                return null;
            }
        }

        public async UniTask SaveCurrentState(string saveName)
        {
            SaveData saveData = CaptureCurrentState();
            if (saveData != null)
            {
                await Save(saveName, saveData);
            }
        }

        private UniTask Save(string saveName, SaveData data)
        {
            if (_provider == null)
            {
                NovelGameDebug.LogError("[SaveLoadService] Provider is not initialized.");
                return UniTask.CompletedTask;
            }

            UniTask<Texture2D> captureTask = CameraSaver.CaptureScreenAndCropAsync(PREVIEW_IMAGE_SIZE);

            return captureTask.ContinueWith(originalTexture =>
            {
                Texture2D textureToSave = originalTexture;

                if (_provider is PlayerPrefsSaveLoadProvider && originalTexture != null)
                {
                    textureToSave = ResizeTexture(originalTexture, WEBGL_PREVIEW_SIZE, WEBGL_PREVIEW_SIZE);
                    Object.Destroy(originalTexture);
                }
                var snapshotService = NovelGame.Instance.GetService<SnapshotService>();
                snapshotService.BindToSlot(saveName);
                return _provider.SaveAsync(saveName, data, textureToSave);

            });
        }

        public UniTask<PreloadSave> LoadPreloadSave(string saveName)
        {
            if (_provider == null)
            {
                NovelGameDebug.LogError("[SaveLoadService] Provider is not initialized.");
                return UniTask.FromResult<PreloadSave>(null);
            }
            return _provider.LoadPreloadSaveAsync(saveName);
        }

        public UniTask<SaveData> LoadSave(string saveName)
        {
            if (_provider == null)
            {
                NovelGameDebug.LogError("[SaveLoadService] Provider is not initialized.");
                return UniTask.FromResult<SaveData>(null);
            }
            return _provider.LoadSaveAsync(saveName);
        }

        public UniTask<IEnumerable<string>> GetAllAvailableSaves()
        {
            if (_provider == null)
            {
                NovelGameDebug.LogError("[SaveLoadService] Provider is not initialized.");
                return UniTask.FromResult(Enumerable.Empty<string>());
            }
            return _provider.GetAllAvailableSavesAsync();
        }

        private Texture2D ResizeTexture(Texture2D source, int newWidth, int newHeight)
        {
            RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
            rt.filterMode = FilterMode.Bilinear;
            RenderTexture.active = rt;

            Graphics.Blit(source, rt);

            Texture2D newTexture = new Texture2D(newWidth, newHeight);
            newTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
            newTexture.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            return newTexture;
        }

        public void LoadDataGraph(DialogueGraph graph, SaveData saveData)
        {
            _currentGraph = graph;
            _originalVariableValues = new Dictionary<string, object>();

            IEnumerable<ISaveProgressNode> nodes = graph.AllNodes
                .Select(x => x.Value)
                .OfType<ISaveProgressNode>();

            IEnumerable<VariableNode> variableNodes = graph.nodes
                .OfType<VariableNode>();

            var globalVariables = NovelGame.Instance.GetService<VariablesContainerService>().GlobalVariables;

            foreach (var node in nodes)
            {
                if (saveData.NodesData.TryGetValue(node.GUID, out var savedData))
                {
                    node.SetDataFromSave(savedData);
                }
                else
                {
                    NovelGameDebug.LogError($"save data for node {node.GUID} not found");
                }
            }

            foreach (var node in variableNodes)
            {
                _originalVariableValues[node.GUID] = node.GetStartValue();
                if (saveData.Variables.TryGetValue(node.GUID, out var savedData))
                {
                    node.SetValue(savedData);
                }
                else
                {
                    NovelGameDebug.LogError($"save data for node {node.GUID} not found");
                }
            }

            foreach (var node in globalVariables.Values)
            {
                _originalVariableValues[node.GUID] = node.GetStartValue();
                if (saveData.GlobalVariables.TryGetValue(node.GUID, out var savedData))
                {
                    node.SetValue(savedData);
                }
                else
                {
                    NovelGameDebug.LogError($"save data for node {node.GUID} not found");
                }
            }

#if UNITY_EDITOR
            graph.OnEndExecute -= RestoreOriginalVariableValues;
            graph.OnEndExecute += RestoreOriginalVariableValues;
#endif
        }

        public void ResetDataGraph(DialogueGraph graph)
        {
            IEnumerable<ISaveProgressNode> nodes = graph.AllNodes
                .Select(x => x.Value)
                .OfType<ISaveProgressNode>();

            foreach (var node in nodes)
            {
                node.ResetSaveBehaviour();
            }
        }

        public Dictionary<string, object> ExtractSaveDataFromGraph(DialogueGraph graph)
        {
            Dictionary<string, object> result = new();
            IEnumerable<ISaveProgressNode> nodes = graph.AllNodes
                .Select(x => x.Value)
                .OfType<ISaveProgressNode>();

            foreach (var node in nodes)
            {
                result.Add(node.GUID, node.GetDataForSave());
            }

            return result;
        }

        public async UniTask<bool> DeleteSave(string saveName)
        {
            if (_provider == null)
            {
                NovelGameDebug.LogError("[SaveLoadService] Provider is not initialized.");
                return false;
            }

            return await _provider.DeleteSaveAsync(saveName);
        }

#if UNITY_EDITOR
        private void RestoreOriginalVariableValues()
        {
            if (_originalVariableValues == null || _currentGraph == null) return;

            IEnumerable<VariableNode> variableNodes = _currentGraph.AllNodes
                .Select(x => x.Value)
                .OfType<VariableNode>();

            foreach (var node in variableNodes)
            {
                if (_originalVariableValues.TryGetValue(node.GUID, out var originalValue))
                {
                    node.SetValue(originalValue);
                }
            }

            NovelGameDebug.Log("Restored original Variable values after graph execution from save.");
            _originalVariableValues.Clear();
            _currentGraph = null;
        }
#endif
    }
}