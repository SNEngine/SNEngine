using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.DialogOnScreenSystem;
using SNEngine.Source.SNEngine.Services;
using UnityEngine;

namespace SNEngine.Source.SNEngine.MessageSystem
{
    //[CreateNodeMenu("Dialog/Message OnScreen Node")]
    public class MessageOnScreenNode : AsyncNode, IDialogOnScreenNode
    {
        [SerializeField, TextArea(5, 20)] private string _text;

        public string GetText() => _text;

        public override void Execute()
        {
            base.Execute();
            
            var service = NovelGame.Instance.GetService<MessageService>();
            service.ShowMessage(this);
        }
        
        public void MarkIsEnd()
        {
            StopTask();
        }
    }
}