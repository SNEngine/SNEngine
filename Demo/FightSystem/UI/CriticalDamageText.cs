using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace CoreGame.FightSystem.UI
{
    public class CriticalDamageText : FloatingText
    {
        [SerializeField] private Color _criticalColor = Color.yellow;
        [SerializeField] private float _duration = 1.2f;
        [SerializeField] private float _moveY = 2f;
        [SerializeField] private Ease _moveEase = Ease.OutQuad;
        [SerializeField] private float _fadeDuration = 0.6f;
        [SerializeField] private Ease _fadeEase = Ease.InQuad;
        [SerializeField] private float _scaleUp = 2f;

        public override UniTask Show(float value, Vector3 startPosition)
        {
            return Animate(value, _criticalColor, _duration, _moveY, _moveEase, _fadeDuration, _fadeEase, startPosition, _scaleUp, "!!");
        }
    }
}