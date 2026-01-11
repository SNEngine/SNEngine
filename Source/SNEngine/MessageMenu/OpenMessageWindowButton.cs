using SNEngine.InputSystem;
using SNEngine.Services;
using SNEngine.Source.SNEngine.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SNEngine.Source.SNEngine.MessageMenu
{
    public class OpenMessageWindowButton : MonoBehaviour, IOpenMessageWindowButton, IPointerDownHandler
    {
        private IInputSystem _inputSystem;

        private void OnEnable()
        {
            _inputSystem = NovelGame.Instance.GetService<InputService>();

            _inputSystem.AddListener(OnMessageHotkey, StandaloneInputEventType.KeyDown, true);
            _inputSystem.AddListener(OnMessageGamepadButton, StandaloneInputEventType.KeyDown, true);
        }
        
        public void ResetState()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            NovelGame.Instance.GetService<MessageMenuWindowService>().Show();
        }

        private void OnMessageHotkey(KeyCode keyCode)
        {
        }

        private void OnMessageGamepadButton(KeyCode arg0)
        {
        }
    }
}