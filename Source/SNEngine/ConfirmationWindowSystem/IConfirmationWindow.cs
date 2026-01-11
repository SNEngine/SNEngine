using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SNEngine.ConfirmationWindowSystem
{
    public interface IConfirmationWindow : IResetable, IShowable, IHidden
    {
        void SetData(string keyTitle, string keyMessage, Sprite icon, ConfirmationWindowButtonType buttonTypes, string defaultTitle = "", string defaultMessage = "");
        UniTask<ConfirmationWindowResult> WaitForConfirmation();
    }
}