using Cysharp.Threading.Tasks;
using SNEngine.CharacterSystem;
using SNEngine.Debugging;
using SNEngine.Source.SNEngine.DialogueSystem;
using SNEngine.Source.SNEngine.Services;
using UnityEngine;

namespace SNEngine.DialogSystem
{
    public class DialogNode : PrinterTextNode, IDialogNode
    {
        [Space] [SerializeField] private Character _character;

        public Character Character => _character;

        public override void Execute()
        {
            if (!_character)
            {
                NovelGameDebug.LogError(
                    $"dialog node {GUID} not has character and skipped. You must set character for dialog works");
                return;
            }

            base.Execute();

            var executorService = NovelGame.Instance.GetService<DialogueExecutorService>();
            _ = executorService.ExecuteNode(this);
        }
    }
}