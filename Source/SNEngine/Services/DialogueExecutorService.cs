using Cysharp.Threading.Tasks;
using SNEngine.DialogOnScreenSystem;
using SNEngine.DialogSystem;
using SNEngine.Services;
using SNEngine.Source.SNEngine.DialogueSystem;
using UnityEngine;

namespace SNEngine.Source.SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Dialogue Executor Service")]
    public class DialogueExecutorService : ServiceBase
    {
        private DialogueUIService _vn;
        private MessageService _msg;

        public override void Initialize()
        {
            _vn = NovelGame.Instance.GetService<DialogueUIService>();
            _msg = NovelGame.Instance.GetService<MessageService>();
        }

        public async UniTask ExecuteNode(IDialogNode node)
        {
            _vn.ShowDialog(node);
        }

        public async UniTask ExecuteNode(IDialogOnScreenNode node)
        {
            _msg.ShowMessage(node);
        }
    }
}