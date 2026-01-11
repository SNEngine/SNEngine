using Cysharp.Threading.Tasks;
using SNEngine.Extensions;
using SNEngine.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace SNEngine.ConfirmationWindowSystem
{
    public class ConfirmationWindow : MonoBehaviour, IConfirmationWindow
    {
        [SerializeField] private UILocalizationText _title;
        [SerializeField] private UILocalizationText _message;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _okButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;

        public void SetData(string keyTitle, string keyMessage, Sprite icon, ConfirmationWindowButtonType buttonTypes, string defaultTitle = "", string defaultMessage = "")
        {
            _title.ChangeKey(keyTitle, defaultTitle);
            _message.ChangeKey(keyMessage, defaultMessage);

            _icon.sprite = icon;
            if (_icon.sprite != null)
            {
                _icon.SetAdaptiveSize();
            }
            else
            {
                _icon.gameObject.SetActive(false);
            }

            ConfigureButtons(buttonTypes);
        }

        private void ConfigureButtons(ConfirmationWindowButtonType buttonTypes)
        {
            _okButton.gameObject.SetActive(buttonTypes.HasFlag(ConfirmationWindowButtonType.Ok));
            _cancelButton.gameObject.SetActive(buttonTypes.HasFlag(ConfirmationWindowButtonType.Cancel));
            _yesButton.gameObject.SetActive(buttonTypes.HasFlag(ConfirmationWindowButtonType.Yes));
            _noButton.gameObject.SetActive(buttonTypes.HasFlag(ConfirmationWindowButtonType.No));
        }

        public async UniTask<ConfirmationWindowResult> WaitForConfirmation()
        {
            var source = new UniTaskCompletionSource<ConfirmationWindowButtonType>();

            ResetState();

            _okButton.onClick.AddListener(() => source.TrySetResult(ConfirmationWindowButtonType.Ok));
            _cancelButton.onClick.AddListener(() => source.TrySetResult(ConfirmationWindowButtonType.Cancel));
            _yesButton.onClick.AddListener(() => source.TrySetResult(ConfirmationWindowButtonType.Yes));
            _noButton.onClick.AddListener(() => source.TrySetResult(ConfirmationWindowButtonType.No));

            ConfirmationWindowButtonType result = await source.Task;
            return new ConfirmationWindowResult(result);
        }

        public void Hide() => gameObject.SetActive(false);
        public void Show() => gameObject.SetActive(true);

        public void ResetState()
        {
            _okButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();
            _yesButton.onClick.RemoveAllListeners();
            _noButton.onClick.RemoveAllListeners();
        }
    }
}