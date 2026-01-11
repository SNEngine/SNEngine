using SNEngine.Services;

namespace SNEngine.DialogOnScreenSystem
{
    public class DialogOnScreenNode : PrinterTextNode, IDialogOnScreenNode
    {
        public override void Execute()
        {
            base.Execute();

            var service = NovelGame.Instance.GetService<DialogueOnScreenService>();

            service.ShowDialog(this);
        }




    }
}
