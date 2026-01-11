using SNEngine.Extensions;
using SNEngine.Localization.Models;
using SNEngine.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SNEngine.Localization.UI
{
    public class LanguageSelectView : MonoBehaviour
    {
        [SerializeField] private Image _imageLanguage;
        [SerializeField] private TextMeshProUGUI _textNameLanguage;
        [SerializeField] private Button _button;
        [SerializeField] private UIClickEventRelay _imageClickRelay;
        [SerializeField] private UIClickEventRelay _textClickRelay;
        [SerializeField] private UIPointerEventRelay _imagePointerRelay;
        [SerializeField] private UIPointerEventRelay _textPointerRelay;
        private string _codeLanguage;
        public event Action<string> OnSelect;
        public event Action<string> OnHover;
        public event Action<string> OnExitPointer;

        private void OnEnable()
        {
            _button.onClick.AddListener(Select);
            if (_imageClickRelay != null)
            {
                _imageClickRelay.OnClick += Select;
            }
            if (_textClickRelay != null)
            {
                _textClickRelay.OnClick += Select;
            }

            if (_imagePointerRelay != null)
            {
                _imagePointerRelay.OnEnter += Hover;
                _imagePointerRelay.OnExit += HoverExit;
            }
            if (_textPointerRelay != null)
            {
                _textPointerRelay.OnEnter += Hover;
                _textPointerRelay.OnExit += HoverExit;
            }
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(Select);
            if (_imageClickRelay != null)
            {
                _imageClickRelay.OnClick -= Select;
            }
            if (_textClickRelay != null)
            {
                _textClickRelay.OnClick -= Select;
            }

            if (_imagePointerRelay != null)
            {
                _imagePointerRelay.OnEnter -= Hover;
                _imagePointerRelay.OnExit -= HoverExit;
            }
            if (_textPointerRelay != null)
            {
                _textPointerRelay.OnEnter -= Hover;
                _textPointerRelay.OnExit -= HoverExit;
            }
        }

        private void Select()
        {
            if (!string.IsNullOrEmpty(_codeLanguage))
            {
                OnSelect?.Invoke(_codeLanguage);
            }
        }

        private void HoverExit()
        {
            OnExitPointer?.Invoke(_codeLanguage);
        }

        private void Hover()
        {
            OnHover?.Invoke(_codeLanguage);
        }

        public void SetData(LanguageMetaData languageMetaData, Sprite icon, string code)
        {
            _textNameLanguage.text = languageMetaData.NameLanguage;
            _codeLanguage = code;
            _imageLanguage.sprite = icon;
            _imageLanguage.SetAdaptiveSize();
        }
    }
}