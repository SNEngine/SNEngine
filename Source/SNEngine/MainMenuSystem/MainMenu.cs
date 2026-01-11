using UnityEngine;
using System;
using UnityEngine.UI;
using SNEngine.Services;
using UnityEngine.Events;
using TMPro;
using SNEngine.SaveSystem;
using SNEngine.InputSystem;
using SNEngine.Audio.UI.Services;

namespace SNEngine.MainMenuSystem
{
    public class MainMenu : MonoBehaviour, IMainMenu
    {
        [SerializeField] private Button _buttonNewGamw;
        [SerializeField] private Button _buttonContinue;
        [SerializeField] private Button _buttonSettings;
        [SerializeField] private Button _buttonQuit;

        private DialogueService _dialogueService;
        private IInputSystem _inputSystem;

        private void Awake()
        {
            if (!_buttonNewGamw)
            {
                throw new NullReferenceException("button new game not seted on main menu script");
            }

            if (!_buttonContinue)
            {
                throw new NullReferenceException("button continue game not seted on main menu script");
            }

            if (!_buttonSettings)
            {
                throw new NullReferenceException("button settings game not seted on main menu script");
            }

#if !UNITY_WEBGL && !UNITY_ANDROID && !UNITY_IOS
            if (!_buttonQuit)
            {
                throw new NullReferenceException("button quit game not seted on main menu script");
            }
#endif

            _dialogueService = NovelGame.Instance.GetService<DialogueService>();
            _inputSystem = NovelGame.Instance.GetService<InputService>();

            Initialize();
        }

        private void Initialize()
        {
            UnityAction[] actions =
            {
                NewGame,
                Continue,
                OpenSettings,
#if !UNITY_WEBGL && !UNITY_ANDROID && !UNITY_IOS
                Exit,
	#endif
            };

            Button[] buttons =
            {
                _buttonNewGamw,
                _buttonContinue,
                _buttonSettings,
#if !UNITY_WEBGL && !UNITY_ANDROID && !UNITY_IOS
                _buttonQuit,
	#endif
            };

            for (int i = 0; i < buttons.Length; i++)
            {
                AddListenerToButton(buttons[i], actions[i]);
            }

#if UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
            Destroy(_buttonQuit.gameObject);
#endif
        }

        protected virtual void OnButtonPress(KeyCode key)
        {
            if (!gameObject.activeSelf) return;

            switch (key)
            {
                case KeyCode.JoystickButton0:
                    NewGame();
                    break;
                case KeyCode.JoystickButton1:
                    Continue();
                    break;
                case KeyCode.JoystickButton7:
                    OpenSettings();
                    break;
#if UNITY_STANDALONE
                case KeyCode.JoystickButton6:
                    Exit();
                    break;
#endif
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
            _inputSystem.RemoveListener(OnButtonPress, GamepadButtonEventType.ButtonDown);
#endif
        }

        public void Show()
        {
            gameObject.SetActive(true);
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
            _inputSystem.AddListener(OnButtonPress, GamepadButtonEventType.ButtonDown);
#endif
        }

        private void AddListenerToButton(Button button, UnityAction action)
        {
            button.onClick.AddListener(action);
        }

        private void NewGame()
        {
            _dialogueService.JumpToStartDialogue();

            Hide();
        }

        private void Continue()
        {
            NovelGame.Instance.GetService<SaveListViewService>().Show();
        }

        private void OpenSettings()
        {
            NovelGame.Instance.GetService<SettingsService>().Show();
        }
#if !UNITY_WEBGL && !UNITY_ANDROID && !UNITY_IOS
        private void Exit()
        {
            Application.Quit();
        }
#endif
    }
}