using Cysharp.Threading.Tasks;
using SNEngine.Extensions;
using SNEngine.Localization;
using SNEngine.Services;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SNEngine.InputWindowSystem
{
    public class InputWindow : MonoBehaviour, IInputWindow
    {
        [SerializeField] private TMP_InputField _input;
        [SerializeField] private Image _icon;
        [SerializeField] private UILocalizationText _title;
        [SerializeField] private Button _confirmButton;
        [SerializeField] private Button _cancelButton;



        public void SetData(string keyTitle, Sprite icon, string defaultTitle)
        {
            _title.ChangeKey(keyTitle);
            _icon.sprite = icon;
            _icon.SetAdaptiveSize();

            if (_title.NotCanTranslite)
            {
                _title.GetComponent<TextMeshProUGUI>().text = defaultTitle;
            }
        }

        public async UniTask<InputWindowResult> WaitInputPlayer()
        {
            var source = new UniTaskCompletionSource<InputWindowButton>();

            _confirmButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();

            _confirmButton.onClick.AddListener(() => source.TrySetResult(InputWindowButton.Ok));
            _cancelButton.onClick.AddListener(() => source.TrySetResult(InputWindowButton.Cancel));

            InputWindowButton button = await source.Task;

            return new InputWindowResult(_input.text, button);
        }

        public void Hide()
        {
            gameObject.SetActive(false);

        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void ResetState()
        {
            _input.text = string.Empty;
            _confirmButton.onClick.RemoveAllListeners();
            _cancelButton.onClick.RemoveAllListeners();
        }
    }
}