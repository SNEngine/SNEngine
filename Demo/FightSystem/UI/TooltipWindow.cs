using CoreGame.FightSystem.Abilities;
using SNEngine.Localization;
using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace CoreGame.FightSystem.UI
{
    public class TooltipWindow : MonoBehaviour
    {
        private const string ABILITY_DESC_KEY_PREFIX = "ability_";
        private const string ABILITY_DESC_KEY_SUFFIX = "_desc";

        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI _textDescription;
        [SerializeField] private TextMeshProUGUI _textCost;
        [SerializeField] private UILocalizationText _localizeComponentDesc;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private RectTransform _energyIconRT;

        [Header("Animation Settings")]
        [SerializeField] private float _animationDuration = 0.2f;
        [SerializeField] private Ease _showEase = Ease.OutBack;
        [SerializeField] private Ease _hideEase = Ease.InBack;
        [SerializeField] private float _startScale = 0f;

        private Vector3 _initialScale;
        private Color _initialBackgroundColor;

        private void Awake()
        {
            _initialScale = transform.localScale;
            transform.localScale = Vector3.one * _startScale;
            _initialBackgroundColor = _backgroundImage.color;
        }

        public void SetAbility(ScriptableAbility ability)
        {
            _localizeComponentDesc.ChangeKey(
                $"{ABILITY_DESC_KEY_PREFIX}{ability.GUID}{ABILITY_DESC_KEY_SUFFIX}"
            );
            _textCost.text = ability.Cost.ToString();


            if (_localizeComponentDesc.NotCanTranslite)
            {
                _textDescription.text = ability.DescriptionAbility;
            }
        }

        private void OnEnable()
        {
            ShowAnimated();
        }

        private void OnDisable()
        {
            HideImmediately();
        }

        private void ShowAnimated()
        {
            transform.DOKill(true);
            transform.localScale = Vector3.one * _startScale;

            Sequence sequence = DOTween.Sequence();
            sequence.SetLink(gameObject);

            Tweener scaleTween = transform.DOScale(_initialScale, _animationDuration)
                                         .SetEase(_showEase);

            sequence.Append(scaleTween);

            Color startColor = _initialBackgroundColor;
            startColor.a = 0f;
            _backgroundImage.color = startColor;
            sequence.Join(_backgroundImage.DOFade(_initialBackgroundColor.a, _animationDuration * 0.5f));

            _energyIconRT.localScale = Vector3.one * _startScale;
            sequence.Join(_energyIconRT.DOScale(Vector3.one, _animationDuration * 0.8f).SetEase(_showEase));
        }

        public void HideAnimated()
        {
            transform.DOKill(true);

            Sequence sequence = DOTween.Sequence();
            sequence.SetLink(gameObject);

            Tweener scaleTween = transform.DOScale(Vector3.one * _startScale, _animationDuration)
                                         .SetEase(_hideEase);

            sequence.Append(scaleTween);

            sequence.Join(_backgroundImage.DOFade(0f, _animationDuration * 0.5f));

            sequence.OnComplete(() =>
            {
                gameObject.SetActive(false);
                transform.localScale = _initialScale;
                _backgroundImage.color = _initialBackgroundColor;
            });
        }

        private void HideImmediately()
        {
            transform.DOKill(true);
            transform.localScale = Vector3.one * _startScale;

            Color currentColor = _backgroundImage.color;
            currentColor.a = _initialBackgroundColor.a;
            _backgroundImage.color = currentColor;
        }
    }
}