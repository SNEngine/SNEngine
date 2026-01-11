using Cysharp.Threading.Tasks;
using SNEngine.Services;
using SNEngine.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.ConfirmationWindowSystem
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Confirmation Window Service")]
    public class ConfirmationWindowService : ServiceBase, IShowable, IHidden
    {
        private IConfirmationWindow _confirmationWindow;

        private const string CONFIRMATION_WINDOW_VANILLA_PATH = "UI/ConfirmationWindow";

        public override void Initialize()
        {
            var ui = NovelGame.Instance.GetService<UIService>();

            var confirmation = ResourceLoader.LoadCustomOrVanilla<ConfirmationWindow>(CONFIRMATION_WINDOW_VANILLA_PATH);

            if (confirmation == null)
            {
                Debug.LogError($"ConfirmationWindow could not be loaded from path: {CONFIRMATION_WINDOW_VANILLA_PATH}");
                return;
            }

            var prefab = Object.Instantiate(confirmation);

            prefab.name = confirmation.name;

            _confirmationWindow = prefab;

            ui.AddElementToUIContainer(prefab.gameObject);

            prefab.gameObject.SetActive(false);
        }

        public void Show()
        {
            _confirmationWindow.Show();
        }

        public void Hide()
        {
            _confirmationWindow.Hide();
        }

        public void SetData(string keyTitle, string keyMessage, Sprite icon, ConfirmationWindowButtonType buttonTypes, string defaultTitle = "", string defaultMessage = "")
        {
            _confirmationWindow.SetData(keyTitle, keyMessage, icon, buttonTypes, defaultTitle, defaultMessage);
        }

        public async UniTask<ConfirmationWindowResult> WaitForConfirmation()
        {
            return await _confirmationWindow.WaitForConfirmation();
        }

        public override void ResetState()
        {
            _confirmationWindow.ResetState();
        }
    }
}