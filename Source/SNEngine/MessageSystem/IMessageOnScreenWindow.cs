using SNEngine.DialogOnScreenSystem;
using TMPro;

namespace SNEngine.Source.SNEngine.MessageSystem
{
    public interface IMessageOnScreenWindow 
    {
        void SetData(IDialogOnScreenNode dialog);
        void StartOutputDialog();
        
        void ResetState();
        void SetFontDialog(TMP_FontAsset font);
    }
}