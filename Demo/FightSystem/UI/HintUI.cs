using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CoreGame.FightSystem.UI
{
    public class HintUI : MonoBehaviour
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TextMeshProUGUI _hintText;
        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private float _baseDisplayDuration = 1.5f; // Базовая задержка показа
        [SerializeField] private float _scaleXDuration = 0.3f;
        [SerializeField] private Ease _scaleXEase = Ease.OutBack;
        [SerializeField] private float _textBlinkSpeed = 0.2f;
        [SerializeField] private Color _blinkColor = Color.yellow;

        // Константа, определяющая дополнительное время на каждое слово
        private const float TIME_PER_WORD = 0.15f;

        private Tween _currentTween;
        private float _initialScaleX;
        private Sequence _blinkTween;
        private Color _initialTextColor;

        private Queue<string> _messageQueue = new Queue<string>();
        private bool _isShowing;

        private void Awake()
        {
            if (_backgroundImage != null)
            {
                var c = _backgroundImage.color;
                c.a = 0f;
                _backgroundImage.color = c;
            }
            if (_hintText != null)
            {
                var c = _hintText.color;
                c.a = 0f;
                _hintText.color = c;
                _initialTextColor = _hintText.color;
            }

            _initialScaleX = transform.localScale.x;
            transform.localScale = new Vector3(0f, transform.localScale.y, transform.localScale.z);
            gameObject.SetActive(false);
        }

        public void ShowHint(string message)
        {
            _messageQueue.Enqueue(message);
            ProcessQueue();
        }

        private void ProcessQueue()
        {
            if (_isShowing || _messageQueue.Count == 0) return;

            _isShowing = true;
            string nextMessage = _messageQueue.Dequeue();

            ShowInternal(nextMessage);
        }

        private void ShowInternal(string message)
        {
            if (_hintText != null)
            {
                _hintText.text = message;
                var c = _initialTextColor;
                c.a = 0f;
                _hintText.color = c;
            }

            _currentTween?.Kill(true);
            _blinkTween?.Kill(true);
            _hintText.color = _initialTextColor;

            gameObject.SetActive(true);
            transform.localScale = new Vector3(0f, transform.localScale.y, transform.localScale.z);

            if (_backgroundImage != null)
            {
                var c = _backgroundImage.color;
                c.a = 0f;
                _backgroundImage.color = c;
            }

            int wordCount = Regex.Matches(message, @"\b\w+\b").Count;
            float calculatedDuration = _baseDisplayDuration + (wordCount * TIME_PER_WORD);

            Sequence showSequence = DOTween.Sequence().SetLink(gameObject);

            showSequence.Append(transform.DOScaleX(_initialScaleX, _scaleXDuration).SetEase(_scaleXEase));

            showSequence.Join(_backgroundImage.DOFade(1f, _fadeDuration).SetEase(Ease.Linear));

            showSequence.Append(_hintText.DOFade(1f, _fadeDuration).SetEase(Ease.Linear));

            showSequence.AppendCallback(() =>
            {
                _blinkTween = DOTween.Sequence().SetLink(gameObject).SetLoops(-1);

                _blinkTween.Append(_hintText.DOColor(_blinkColor, _textBlinkSpeed).SetEase(Ease.Linear));

                _blinkTween.Join(_hintText.DOFade(0.3f, _textBlinkSpeed).SetEase(Ease.Linear));

                _blinkTween.Append(_hintText.DOColor(_initialTextColor, _textBlinkSpeed).SetEase(Ease.Linear));
                _blinkTween.Join(_hintText.DOFade(1f, _textBlinkSpeed).SetEase(Ease.Linear));
            });

            Sequence hideSequence = DOTween.Sequence().SetLink(gameObject);

            hideSequence.AppendInterval(calculatedDuration);

            hideSequence.AppendCallback(() =>
            {
                _blinkTween?.Kill();
                _hintText.color = _initialTextColor;
            });

            hideSequence.Append(_hintText.DOFade(0f, _fadeDuration).SetEase(Ease.Linear));

            hideSequence.Join(_backgroundImage.DOFade(0f, _fadeDuration).SetEase(Ease.Linear));

            hideSequence.Join(transform.DOScaleX(0f, _scaleXDuration).SetEase(Ease.Linear));

            hideSequence.OnComplete(() =>
            {
                gameObject.SetActive(false);
                _isShowing = false;
                ProcessQueue();
            });

            _currentTween = DOTween.Sequence().Append(showSequence).Append(hideSequence);
        }
    }
}