using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace CoreGame.FightSystem.UI
{

    [RequireComponent(typeof(Image))]
    public class FillSlider : MonoBehaviour
    {
        [SerializeField]
        private float _value;

        [SerializeField, Min(0)]
        private float _maxValue = 1;

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
                _valueTweener?.Kill();
                _value = Mathf.Clamp(value, 0, _maxValue);
                UpdateFill();
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
                                           _value = x;
                                           UpdateFill();
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

        private void OnValidate()
        {
            _maxValue = Mathf.Max(0, _maxValue);
            _value = Mathf.Clamp(_value, 0, _maxValue);
            UpdateFill();
        }
    }

}