using SNEngine.CharacterSystem;

namespace SNEngine.Source.SNEngine.DialogueSystem
{
    public interface IDialogNode : IPrinterNode
    {
        Character Character { get; }
    }
}
