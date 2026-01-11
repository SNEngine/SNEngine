using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace CoreGame.FightSystem.UI
{
    public class HealText : FloatingText
    {
        [SerializeField] private Color _healColor = Color.green;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _moveY = 1.5f;
        [SerializeField] private Ease _moveEase = Ease.OutQuad;
        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private Ease _fadeEase = Ease.InQuad;
        [SerializeField] private float _scaleUp = 1.5f;

        public override UniTask Show(float value, Vector3 startPosition)
        {
            return Animate(value, _healColor, _duration, _moveY, _moveEase, _fadeDuration, _fadeEase, startPosition, _scaleUp);
        }
    }
}