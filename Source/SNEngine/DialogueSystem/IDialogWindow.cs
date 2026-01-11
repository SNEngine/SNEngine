using SNEngine.Source.SNEngine.DialogueSystem;

namespace SNEngine.DialogSystem
{
    public interface IDialogWindow : IHidden, IShowable, IResetable, IPrinterText, IPrinterDialogueText, IPrinterTalkingCharacter
    {
        void SetData(IDialogNode dialogNode);
    }
}
