using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.DialogSystem;
using SNEngine.Extensions;
using SNEngine.SaveSystem;
using SNEngine.Services;
using UnityEngine;
using SaveData = SNEngine.SaveSystem.Models.SaveData;

namespace SNEngine.Graphs
{
    [CreateAssetMenu(menuName = "SNEngine/New Dialogue Graph")]
    public class DialogueGraph : BaseGraph, IDialogue
    {
        public object Name => name;

        public BaseNode CurrentExecuteNode => Queue.Current;

        public override void Execute()
        {
            NovelGame.Instance.GetService<LanguageService>().TransliteGraph(this);
            NovelGame.Instance.GetService<SaveLoadService>().ResetDataGraph(this);
            base.Execute();
        }

        public override string GetWindowTitle()
        {
            return name;
        }

        public bool HasNextDialogueOnExit()
        {
            return Queue.HasNextDialogueOnExit();
        }

        public void LoadSave (string nodeGuid, SaveData saveData)
        {
            NovelGame.Instance.GetService<SaveLoadService>().LoadDataGraph(this, saveData);
            NovelGame.Instance.GetService<LanguageService>().TransliteGraph(this);
            base.JumptToNode(nodeGuid);
        }

       
    }
}
