using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SNEngine.UI
{
    [RequireComponent(typeof(Image))]
    public class SimpleButtonTransparency : UIAnimateButtonBase
    {
        [SerializeField, ReadOnly] private Image _image;
        [SerializeField, Range(0f, 1f)] private float _defaultAlpha = 0.5f;
        [SerializeField, Range(0f, 1f)] private float _hoverAlpha = 1.0f;
        [SerializeField] private float _animationDuration = 0.2f;

        private void Awake()
        {
            Color initialColor = _image.color;
            initialColor.a = _defaultAlpha;
            _image.color = initialColor;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            AnimateHover(true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            AnimateHover(false);
        }

        protected override void AnimateHover(bool isHovering)
        {
            float targetAlpha = isHovering ? _hoverAlpha : _defaultAlpha;
            _image.DOComplete();
            _image.DOFade(targetAlpha, _animationDuration).SetEase(Ease.OutSine);
        }

        private void OnValidate()
        {
            if (!_image)
            {
                _image = GetComponent<Image>();
            }
        }
    }
}