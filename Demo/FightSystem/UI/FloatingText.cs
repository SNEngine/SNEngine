using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions.Attributes;

namespace CoreGame.FightSystem.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public abstract class FloatingText : MonoBehaviour
    {
        [SerializeField, ReadOnly(ReadOnlyMode.Always)] private TextMeshProUGUI _component;
        protected TextMeshProUGUI Component => _component;

        public abstract UniTask Show(float value, Vector3 startPosition);
        public virtual UniTask Show(float value, Vector3 startPosition, string textSuffix)
        {
            return Animate(value, Component.color, 1f, 1.5f, Ease.OutQuad, 0.5f, Ease.InQuad, startPosition, 1f, textSuffix);
        }

        protected async UniTask Animate(float value, Color startColor, float duration, float moveY, Ease moveEase, float fadeDuration, Ease fadeEase, Vector3 startPosition, float scaleUp = 1f, string textSuffix = "")
        {
            transform.position = startPosition;
            _component.text = Mathf.RoundToInt(value).ToString() + textSuffix;
            _component.color = startColor;
            transform.localScale = Vector3.one * 0.1f;
            gameObject.SetActive(true);

            DOTween.Kill(transform);
            DOTween.Kill(_component);

            Vector3 endPosition = startPosition + new Vector3(0, moveY, 0);

            UniTask moveTask = transform.DOMove(endPosition, duration)
                .SetEase(moveEase)
                .ToUniTask();

            UniTask scaleTask = transform.DOScale(Vector3.one * scaleUp, duration / 3)
                .SetEase(Ease.OutBack)
                .ToUniTask();

            await UniTask.WhenAll(moveTask, scaleTask);

            await _component.DOFade(0, fadeDuration)
                .SetEase(fadeEase)
                .ToUniTask();

            transform.DOKill();
            _component.DOKill();

            gameObject.SetActive(false);
        }

        private void OnValidate()
        {
            if (!_component)
            {
                _component = GetComponent<TextMeshProUGUI>();
            }
        }
    }
}