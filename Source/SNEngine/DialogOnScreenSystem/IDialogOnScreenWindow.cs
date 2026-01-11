using SNEngine.DialogSystem;

namespace SNEngine.DialogOnScreenSystem
{
    public interface IDialogOnScreenWindow : IPrinterText, IPrinterDialogueText, IShowable, IHidden
    {
        void SetData(IDialogOnScreenNode dialog);
    }
}
