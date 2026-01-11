using SNEngine.InputSystem;
using SNEngine.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SNEngine.PauseWindowSystem
{
    public class OpenPauseWindowButton : MonoBehaviour, IOpenPauseWindowButton, IPointerDownHandler
    {
        private IInputSystem _inputSystem;

        private void OnEnable()
        {
            _inputSystem = NovelGame.Instance.GetService<InputService>();

            _inputSystem.AddListener(OnPauseHotkey, StandaloneInputEventType.KeyDown, true);
            _inputSystem.AddListener(OnPauseGamepadButton, GamepadButtonEventType.ButtonDown, true);
        }

        private void OnDisable()
        {
            if (_inputSystem != null)
            {
                _inputSystem.RemoveListener(OnPauseHotkey, StandaloneInputEventType.KeyDown);
                _inputSystem.RemoveListener(OnPauseGamepadButton, GamepadButtonEventType.ButtonDown);
            }
        }

        private void OnPauseHotkey(KeyCode keyCode)
        {
            if (keyCode == KeyCode.Escape)
            {
                Pause();
            }
        }

        private void OnPauseGamepadButton(KeyCode keyCode)
        {
            if (keyCode == KeyCode.JoystickButton7)
            {
                Pause();
            }
        }

        private void Pause()
        {
            NovelGame.Instance.GetService<InputService>().SetActiveInput(false);
            NovelGame.Instance.GetService<PauseWindowService>().Show();
        }

        public void ResetState()
        {
            gameObject.SetActive(false);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Pause();
        }
    }
}