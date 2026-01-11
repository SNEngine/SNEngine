using System;
using SNEngine.Services;
using UnityEngine;
using UnityEngine.UI;

namespace SNEngine.Source.SNEngine.MessageMenu
{
    public class MessageMenuWindow : MonoBehaviour, IMessageMenuWindow
    {
        [SerializeField] private Button _dialogueHistoryButton;
        [SerializeField] private Button _closeButton;

        private InputService _inputService;
        
        private Button[] _buttons;
        private float _currentTimeScale;

        private void OnEnable()
        {
            _currentTimeScale = Time.timeScale;
            Time.timeScale = 0;
            
            _inputService = NovelGame.Instance.GetService<InputService>();

            _buttons = new Button[]
            {
                _dialogueHistoryButton,
                _closeButton
            };

            foreach (var button in _buttons)
            {
                button.onClick.RemoveAllListeners();
            }

            _closeButton.onClick.AddListener(CloseMenu);
            _inputService.SetActiveInput(false);
        }

        private void OnDisable()
        {
            _inputService.SetActiveInput(true);
            Time.timeScale = _currentTimeScale;
        }

        private void CloseMenu()
        {
            gameObject.SetActive(false);
        }

        public void ResetState()
        {
            throw new NotImplementedException();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}