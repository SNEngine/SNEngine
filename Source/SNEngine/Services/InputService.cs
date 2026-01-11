using SNEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Input Service")]
    public class InputService : ServiceBase, IInputSystem
    {
        private IInputSystem _input;
        public override void Initialize()
        {
            var input = Resources.Load<InputSystem.InputSystem>("System/Input/InputSystem");

            var prefab = Object.Instantiate(input);

            prefab.name = input.name;

            Object.DontDestroyOnLoad(prefab);

            _input = prefab;
        }

        public void AddListener(UnityAction<KeyCode> action, StandaloneInputEventType eventType)
        {
            _input.AddListener(action, eventType);
        }

        public void AddListener(UnityAction<KeyCode> action, StandaloneInputEventType eventType, bool isHighPriority)
        {
            _input.AddListener(action, eventType, isHighPriority);
        }

        public void RemoveListener(UnityAction<KeyCode> action, StandaloneInputEventType eventType)
        {
            _input.RemoveListener(action, eventType);
        }

        public void AddListener(UnityAction<Touch> action, MobileInputEventType eventType)
        {
            _input.AddListener(action, eventType);
        }

        public void AddListener(UnityAction<Touch> action, MobileInputEventType eventType, bool isHighPriority)
        {
            _input.AddListener(action, eventType, isHighPriority);
        }

        public void RemoveListener(UnityAction<Touch> action, MobileInputEventType eventType)
        {
            _input.RemoveListener(action, eventType);
        }

        public void AddListener(UnityAction<KeyCode> action, GamepadButtonEventType eventType)
        {
            _input.AddListener(action, eventType);
        }

        public void AddListener(UnityAction<KeyCode> action, GamepadButtonEventType eventType, bool isHighPriority)
        {
            _input.AddListener(action, eventType, isHighPriority);
        }

        public void RemoveListener(UnityAction<KeyCode> action, GamepadButtonEventType eventType)
        {
            _input.RemoveListener(action, eventType);
        }

        public void AddAxisListener(UnityAction<string, float> action, string axisName)
        {
            _input.AddAxisListener(action, axisName);
        }

        public void RemoveAxisListener(UnityAction<string, float> action)
        {
            _input.RemoveAxisListener(action);
        }

        public void SetActiveInput(bool status)
        {
            _input.SetActiveInput(status);
        }
    }
}