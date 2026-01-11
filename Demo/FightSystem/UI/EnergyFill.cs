using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using UnityEngine;

namespace CoreGame.FightSystem.UI
{
    public class EnergyFill : MonoBehaviour
    {
        [SerializeField, ReadOnly] private FillSlider _fill;
        [SerializeField] private Ease _easeAnimation = Ease.Flash;
        [SerializeField, Min(0)] private float _durationAnimation = 0.3f;

        public void SetStateEmpty ()
        {
            _fill.SetValueSmoothly(0, _durationAnimation, _easeAnimation);
        }

        public void SetStateFull()
        {
            _fill.SetValueSmoothly(1, _durationAnimation, _easeAnimation);
        }

        private void OnValidate()
        {
            if (!_fill)
            {
                _fill = GetComponentInChildren<FillSlider>();
            }
        }
    }
}
