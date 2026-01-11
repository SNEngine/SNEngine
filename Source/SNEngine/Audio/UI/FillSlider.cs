using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace SNEngine.Audio.UI
{

    [RequireComponent(typeof(Image))]
    public class FillSlider : MonoBehaviour, IDragHandler, IPointerClickHandler
    {
        [Header("Events")]
        public UnityEvent<float> OnValueChanged = new UnityEvent<float>();

        [SerializeField]
        private float _value;

        [SerializeField, Min(0)]
        private float _maxValue = 1;

        [SerializeField]
        private bool _canBeDragged = true;

        [Header("DOTween Settings")]
        [SerializeField]
        private float _smoothDuration = 0.3f;

        private Tweener _valueTweener;

        private Image _fillImage;

        public float Value
        {
            get => _value;
            set
            {
                if (_value.Equals(value)) return;

                _valueTweener?.Kill();
                _value = Mathf.Clamp(value, 0, _maxValue);
                UpdateFill();

                OnValueChanged?.Invoke(_value);
            }
        }

        public float MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = Mathf.Max(0, value);
                _value = Mathf.Clamp(_value, 0, _maxValue);
                UpdateFill();
            }
        }

        public bool CanBeDragged
        {
            get => _canBeDragged;
            set => _canBeDragged = value;
        }

        private void Awake()
        {
            _fillImage = GetComponent<Image>();
            UpdateFill();
        }

        public void SetValueSmoothly(float targetValue, float? duration = null, Ease ease = Ease.OutSine)
        {
            float clampedTarget = Mathf.Clamp(targetValue, 0, _maxValue);
            float finalDuration = duration ?? _smoothDuration;

            if (_value.Equals(clampedTarget) && (_valueTweener == null || !_valueTweener.IsActive()))
            {
                return;
            }

            _valueTweener?.Kill();

            _valueTweener = DOTween.To(() => _value,
                                       x =>
                                       {
                                           if (x.Equals(_value)) return;
                                           _value = x;
                                           UpdateFill();
                                           OnValueChanged?.Invoke(_value);
                                       },
                                       clampedTarget,
                                       finalDuration)
                               .SetEase(ease);

        }

        private void UpdateFill()
        {
            if (!_fillImage)
            {
                _fillImage = GetComponent<Image>();
                if (!_fillImage) return;
            }

            _value = Mathf.Clamp(_value, 0, _maxValue);

            _fillImage.fillAmount = (_maxValue > 0) ? (_value / _maxValue) : 0;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_canBeDragged || _fillImage.type != Image.Type.Filled) return;

            UpdateValueFromPointer(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_canBeDragged || _fillImage.type != Image.Type.Filled) return;

            UpdateValueFromPointer(eventData);
        }

        private void UpdateValueFromPointer(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _fillImage.rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            float normalizedValue;

            switch (_fillImage.fillMethod)
            {
                case Image.FillMethod.Horizontal:
                    normalizedValue = (localPoint.x - _fillImage.rectTransform.rect.x) / _fillImage.rectTransform.rect.width;
                    break;
                case Image.FillMethod.Vertical:
                    normalizedValue = (localPoint.y - _fillImage.rectTransform.rect.y) / _fillImage.rectTransform.rect.height;
                    break;
                default:
                    return;
            }

            Value = normalizedValue * _maxValue;
        }

        private void OnValidate()
        {
            _maxValue = Mathf.Max(0, _maxValue);
            _value = Mathf.Clamp(_value, 0, _maxValue);
            UpdateFill();
        }
    }

}