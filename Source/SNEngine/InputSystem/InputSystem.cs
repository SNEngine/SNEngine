using SNEngine.Debugging;
using SNEngine.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SNEngine.InputSystem
{
    public class InputSystem : MonoBehaviour, IInputSystem
    {
        #region Listener Lists
#if UNITY_STANDALONE || UNITY_WEBGL
        private readonly List<UnityAction<KeyCode>> _onKeyUp = new();
        private readonly List<UnityAction<KeyCode>> _onKeyDown = new();
        private readonly List<UnityAction<KeyCode>> _onKey = new();
        private Array _keyCodes;
#endif

#if UNITY_ANDROID || UNITY_IOS
        private readonly List<UnityAction<Touch>> _onTouchBegan = new();
        private readonly List<UnityAction<Touch>> _onTouchCanceled = new();
        private readonly List<UnityAction<Touch>> _onTouchEnded = new();
        private readonly List<UnityAction<Touch>> _onTouchMoved = new();
        private readonly List<UnityAction<Touch>> _onTouchStationary = new();
#endif

#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
        private readonly List<UnityAction<KeyCode>> _onButtonDown = new();
        private readonly List<UnityAction<KeyCode>> _onButtonUp = new();
        private readonly List<UnityAction<KeyCode>> _onButton = new();
        private readonly List<UnityAction<string, float>> _onAxisChanged = new();

        private Dictionary<string, float> _axisValues = new Dictionary<string, float>();
        private List<KeyCode> _gamepadButtonCodes;
        private bool _enabledInput = true;
#endif
        #endregion

        private void Awake()
        {
#if UNITY_STANDALONE || UNITY_WEBGL
            _keyCodes = Enum.GetValues(typeof(KeyCode));
            Log("Enabled Standalone Input");
#endif

#if UNITY_ANDROID || UNITY_IOS
            Log("Enabled Mobile Input");
#endif

#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
            _gamepadButtonCodes = new List<KeyCode>();
            for (int i = 0; i <= 19; i++)
            {
                if (Enum.IsDefined(typeof(KeyCode), "JoystickButton" + i))
                {
                    _gamepadButtonCodes.Add((KeyCode)Enum.Parse(typeof(KeyCode), "JoystickButton" + i));
                }
            }
            Log("Enabled Gamepad Input");
#endif
        }


        private void LateUpdate()
        {
            if (!_enabledInput) return;

            #region Standalone Input
#if UNITY_STANDALONE || UNITY_WEBGL
            ListeringKeyCodes();
#endif
            #endregion

            #region Mobile Input
#if UNITY_ANDROID || UNITY_IOS
            ListeringTouch();
#endif
            #endregion

            #region Gamepad Input
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
            ListeringButtons();
            ListeringAxis();
#endif
            #endregion
        }

        private void InvokeSafe<T>(List<UnityAction<T>> listeners, T data)
        {
            if (!_enabledInput || listeners.Count == 0) return;

            var activeListeners = new List<UnityAction<T>>(listeners);

            foreach (var action in activeListeners)
            {
                if (!_enabledInput)
                {
                    NovelGameDebug.Log($"InputSystem: Input disabled during event loop. Stopping propagation for event: {data}");
                    break;
                }

                try
                {
                    action?.Invoke(data);
                }
                catch (Exception e)
                {
                    NovelGameDebug.LogError($"Error in Input Listener: {e}");
                }
            }
        }

        private void InvokeSafe(List<UnityAction<string, float>> listeners, string axis, float value)
        {
            if (!_enabledInput || listeners.Count == 0) return;
            var activeListeners = new List<UnityAction<string, float>>(listeners);

            foreach (var action in activeListeners)
            {
                if (!_enabledInput) break;

                try
                {
                    action?.Invoke(axis, value);
                }
                catch (Exception e)
                {
                    NovelGameDebug.LogError($"Error in Axis Listener: {e}");
                }
            }
        }

#if UNITY_STANDALONE || UNITY_WEBGL
        private void ListeringKeyCodes()
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in _keyCodes)
                {
                    if (!_enabledInput) return;

                    if (Input.GetKeyDown(keyCode))
                    {
                        InvokeSafe(_onKeyDown, keyCode);
                    }
                }
            }

            if (Input.anyKey)
            {
                foreach (KeyCode keyCode in _keyCodes)
                {
                    if (!_enabledInput) return;

                    if (Input.GetKey(keyCode))
                    {
                        InvokeSafe(_onKey, keyCode);
                    }

                    if (!_enabledInput) return;

                    if (Input.GetKeyUp(keyCode))
                    {
                        InvokeSafe(_onKeyUp, keyCode);
                    }
                }
            }
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        private void ListeringTouch()
        {
            if (Input.touchCount > 0)
            {
                var touches = Input.touches;
                foreach (var touch in touches)
                {
                    if (!_enabledInput) return;
                    
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            InvokeSafe(_onTouchBegan, touch);
                            break;
                        case TouchPhase.Moved:
                            if (!_enabledInput) return;
                            InvokeSafe(_onTouchMoved, touch);
                            break;
                        case TouchPhase.Stationary:
                            if (!_enabledInput) return;
                            InvokeSafe(_onTouchStationary, touch);
                            break;
                        case TouchPhase.Ended:
                            if (!_enabledInput) return;
                            InvokeSafe(_onTouchEnded, touch);
                            break;
                        case TouchPhase.Canceled:
                            if (!_enabledInput) return;
                            InvokeSafe(_onTouchCanceled, touch);
                            break;
                    }
                }
            }
        }

#endif

#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
        private void ListeringButtons()
        {
            foreach (var keyCode in _gamepadButtonCodes)
            {
                if (!_enabledInput) return;

                if (Input.GetKeyDown(keyCode))
                {
                    InvokeSafe(_onButtonDown, keyCode);
                }

                if (!_enabledInput) return;

                if (Input.GetKey(keyCode))
                {
                    InvokeSafe(_onButton, keyCode);
                }

                if (!_enabledInput) return;

                if (Input.GetKeyUp(keyCode))
                {
                    InvokeSafe(_onButtonUp, keyCode);
                }
            }
        }

        private void ListeringAxis()
        {
            if (_onAxisChanged.Count == 0) return;

            var axesToUpdate = new List<string>(_axisValues.Keys);

            foreach (var axisName in axesToUpdate)
            {
                if (!_enabledInput) return;

                float currentValue = Input.GetAxis(axisName);
                if (Mathf.Abs(currentValue - _axisValues[axisName]) > float.Epsilon)
                {
                    _axisValues[axisName] = currentValue;
                    InvokeSafe(_onAxisChanged, axisName, currentValue);
                }
            }
        }
#endif

        private void AddToList<T>(List<UnityAction<T>> list, UnityAction<T> action, bool isHighPriority)
        {
            if (list.Contains(action)) return;

            if (isHighPriority)
            {
                list.Insert(0, action);
            }
            else
            {
                list.Add(action);
            }
        }

        public void AddListener(UnityAction<KeyCode> action, StandaloneInputEventType eventType)
        {
            AddListener(action, eventType, false);
        }

        public void AddListener(UnityAction<KeyCode> action, StandaloneInputEventType eventType, bool isHighPriority)
        {
#if UNITY_STANDALONE || UNITY_WEBGL
            List<UnityAction<KeyCode>> targetList = eventType switch
            {
                StandaloneInputEventType.KeyDown => _onKeyDown,
                StandaloneInputEventType.KeyUp => _onKeyUp,
                StandaloneInputEventType.KeyPressing => _onKey,
                _ => null
            };

            if (targetList != null)
            {
                AddToList(targetList, action, isHighPriority);
                Log(action, eventType, nameof(AddListener));
            }
#endif
        }

        public void RemoveListener(UnityAction<KeyCode> action, StandaloneInputEventType eventType)
        {
#if UNITY_STANDALONE || UNITY_WEBGL
            List<UnityAction<KeyCode>> targetList = eventType switch
            {
                StandaloneInputEventType.KeyDown => _onKeyDown,
                StandaloneInputEventType.KeyUp => _onKeyUp,
                StandaloneInputEventType.KeyPressing => _onKey,
                _ => null
            };

            if (targetList != null && targetList.Contains(action))
            {
                targetList.Remove(action);
                Log(action, eventType, nameof(RemoveListener));
            }
#endif
        }

        public void AddListener(UnityAction<Touch> action, MobileInputEventType eventType)
        {
            AddListener(action, eventType, false);
        }

        public void AddListener(UnityAction<Touch> action, MobileInputEventType eventType, bool isHighPriority)
        {
#if UNITY_ANDROID || UNITY_IOS
            List<UnityAction<Touch>> targetList = eventType switch
            {
                MobileInputEventType.TouchBegan => _onTouchBegan,
                MobileInputEventType.TouchCanceled => _onTouchCanceled,
                MobileInputEventType.TouchEnded => _onTouchEnded,
                MobileInputEventType.TouchMoved => _onTouchMoved,
                MobileInputEventType.TouchStationary => _onTouchStationary,
                _ => null
            };

            if (targetList != null)
            {
                AddToList(targetList, action, isHighPriority);
                Log(action, eventType, nameof(AddListener));
            }
#endif
        }

        public void RemoveListener(UnityAction<Touch> action, MobileInputEventType eventType)
        {
#if UNITY_ANDROID || UNITY_IOS
            List<UnityAction<Touch>> targetList = eventType switch
            {
                MobileInputEventType.TouchBegan => _onTouchBegan,
                MobileInputEventType.TouchCanceled => _onTouchCanceled,
                MobileInputEventType.TouchEnded => _onTouchEnded,
                MobileInputEventType.TouchMoved => _onTouchMoved,
                MobileInputEventType.TouchStationary => _onTouchStationary,
                _ => null
            };

            if (targetList != null && targetList.Contains(action))
            {
                targetList.Remove(action);
                Log(action, eventType, nameof(RemoveListener));
            }
#endif
        }

        public void AddListener(UnityAction<KeyCode> action, GamepadButtonEventType eventType)
        {
            AddListener(action, eventType, false);
        }

        public void AddListener(UnityAction<KeyCode> action, GamepadButtonEventType eventType, bool isHighPriority)
        {
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
            List<UnityAction<KeyCode>> targetList = eventType switch
            {
                GamepadButtonEventType.ButtonDown => _onButtonDown,
                GamepadButtonEventType.ButtonUp => _onButtonUp,
                GamepadButtonEventType.ButtonPressing => _onButton,
                _ => null
            };

            if (targetList != null)
            {
                AddToList(targetList, action, isHighPriority);
                Log(action, eventType, nameof(AddListener));
            }
#endif
        }

        public void RemoveListener(UnityAction<KeyCode> action, GamepadButtonEventType eventType)
        {
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
            List<UnityAction<KeyCode>> targetList = eventType switch
            {
                GamepadButtonEventType.ButtonDown => _onButtonDown,
                GamepadButtonEventType.ButtonUp => _onButtonUp,
                GamepadButtonEventType.ButtonPressing => _onButton,
                _ => null
            };

            if (targetList != null && targetList.Contains(action))
            {
                targetList.Remove(action);
                Log(action, eventType, nameof(RemoveListener));
            }
#endif
        }

        public void AddAxisListener(UnityAction<string, float> action, string axisName)
        {
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
            if (!_onAxisChanged.Contains(action))
            {
                _onAxisChanged.Add(action);
                if (!_axisValues.ContainsKey(axisName))
                {
                    _axisValues.Add(axisName, 0f);
                }
                Log(action.Target.GetType().Name, axisName, nameof(AddAxisListener));
            }
#endif
        }

        public void RemoveAxisListener(UnityAction<string, float> action)
        {
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
            if (_onAxisChanged.Contains(action))
            {
                _onAxisChanged.Remove(action);
                Log(action.Target.GetType().Name, "Axis Listener", nameof(RemoveAxisListener));
            }
#endif
        }

#if UNITY_STANDALONE || UNITY_WEBGL
        private void Log(UnityAction<KeyCode> action, StandaloneInputEventType eventType, string message)
        {
            Log($"{message}:Target Event: <b>On{eventType}</b> Observer: <b>{action.Target.GetType().Name}</b>");
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        private void Log(UnityAction<Touch> action, MobileInputEventType eventType, string message)
        {
            Log($"{message}:Target Event: <b>On{eventType}</b> Observer: <b>{action.Target.GetType().Name}</b>");
        }
#endif

#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
        private void Log(UnityAction<KeyCode> action, GamepadButtonEventType eventType, string message)
        {
            Log($"{message}:Target Event: <b>On{eventType}</b> Observer: <b>{action.Target.GetType().Name}</b>");
        }

        private void Log(string observerName, string eventType, string message)
        {
            Log($"{message}:Target Event: <b>{eventType}</b> Observer: <b>{observerName}</b>");
        }
#endif

        private void Log(string message)
        {
            NovelGameDebug.Log($"<color=#baa229>{nameof(InputSystem)}:</color> <b>{message}</b>.");
        }

        public void SetActiveInput(bool status)
        {
            Log($"Active input flag changed: {status}");
            _enabledInput = status;
        }
    }
}