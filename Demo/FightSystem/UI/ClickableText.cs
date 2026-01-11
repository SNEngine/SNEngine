using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions.Attributes;

namespace CoreGame.FightSystem.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ClickableText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public delegate void ClickAction();
        public event ClickAction OnClick;

        [SerializeField] private bool _isInteractable = true;
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _hoverColor = Color.yellow;
        [SerializeField] private Color _pressedColor = Color.gray;
        [SerializeField] private Color _disabledColor = Color.gray;
        [SerializeField] private float _transitionDuration = 0.1f;
        [SerializeField] private float _hoverScale = 1.05f;
        [SerializeField] private float _pressedScale = 0.95f;

        [SerializeField, ReadOnly(ReadOnlyMode.Always)] private TextMeshProUGUI _textComponent;
        private Vector3 _originalScale;
        private bool _isPointerOver = false;

        public bool Interactable
        {
            get => _isInteractable;
            set
            {
                _isInteractable = value;
                if (Application.isPlaying)
                {
                    DOTween.Kill(transform);
                    DOTween.Kill(_textComponent);
                    _textComponent.color = _isInteractable ? _normalColor : _disabledColor;
                    transform.localScale = _originalScale;
                }
                else
                {
                    OnValidate();
                }
            }
        }

        protected TextMeshProUGUI Component => _textComponent;

        private void Awake()
        {
            _originalScale = transform.localScale;
            _textComponent.color = _isInteractable ? _normalColor : _disabledColor;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isInteractable) return;
            _isPointerOver = true;

            _textComponent.DOColor(_hoverColor, _transitionDuration);
            transform.DOScale(_originalScale * _hoverScale, _transitionDuration).SetEase(Ease.OutSine);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!_isInteractable) return;
            _isPointerOver = false;

            _textComponent.DOColor(_normalColor, _transitionDuration);
            transform.DOScale(_originalScale, _transitionDuration).SetEase(Ease.OutSine);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (!_isInteractable) return;

            _textComponent.DOColor(_pressedColor, _transitionDuration);
            transform.DOScale(_originalScale * _pressedScale, _transitionDuration).SetEase(Ease.OutSine);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (!_isInteractable) return;

            if (_isPointerOver)
            {
                _textComponent.DOColor(_hoverColor, _transitionDuration);
                transform.DOScale(_originalScale * _hoverScale, _transitionDuration).SetEase(Ease.OutSine);
            }
            else
            {
                _textComponent.DOColor(_normalColor, _transitionDuration);
                transform.DOScale(_originalScale, _transitionDuration).SetEase(Ease.OutSine);
            }
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!_isInteractable) return;
            OnClick?.Invoke();
        }

        public void AddListener(ClickAction action)
        {
            OnClick += action;
        }

        public void RemoveListener(ClickAction action)
        {
            OnClick -= action;
        }

        protected virtual void OnDisable()
        {
            if (Application.isPlaying)
            {
                DOTween.Kill(transform);
                DOTween.Kill(_textComponent);
            }
            transform.localScale = _originalScale;
            _textComponent.color = _isInteractable ? _normalColor : _disabledColor;
        }

        protected virtual void OnValidate()
        {
            if (!_textComponent)
            {
                _textComponent = GetComponent<TextMeshProUGUI>();
            }

            if (_textComponent != null && !Application.isPlaying)
            {
                _textComponent.color = _isInteractable ? _normalColor : _disabledColor;
            }
        }
    }
}