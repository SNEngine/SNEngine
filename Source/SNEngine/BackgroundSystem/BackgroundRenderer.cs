using Cysharp.Threading.Tasks;
using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using UnityEngine;
using SNEngine.Extensions;
using SNEngine.Animations;

namespace SNEngine.BackgroundSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BackgroundRenderer : MonoBehaviour, IBackgroundRenderer
    {
        public bool UseTransition { get; set; }

        [SerializeField] private SpriteRenderer maskTransition;

        private Sprite oldSetedBackground;

        [SerializeField, ReadOnly(ReadOnlyMode.Always)] private SpriteRenderer spriteRenderer;
        private Tween currentTween;
        protected SpriteRenderer SpriteRenderer => spriteRenderer;

        public void SetData(Sprite data)
        {
            if (maskTransition != null)
            {
                oldSetedBackground = spriteRenderer.sprite;

                maskTransition.sprite = oldSetedBackground;
            }

            UpdateBackground(data).Forget();
        }

        private async UniTask UpdateBackground(Sprite data)
        {
            await UniTask.WaitForEndOfFrame(this);

            spriteRenderer.sprite = data;
        }

        public void Clear()
        {
            spriteRenderer.sprite = null;
        }

        private void OnValidate()
        {
            if (!spriteRenderer)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        public void ResetState()
        {
            Clear();
            spriteRenderer.color = Color.white;
            transform.position = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;
            spriteRenderer.sprite = null;
            currentTween?.Kill();
        }

        #region Animations

        public async UniTask SetTransperent(float fadeValue, float duration, Ease ease)
        {
            currentTween = spriteRenderer.DOFade(fadeValue, duration).SetEase(ease);
            await currentTween;
        }

        public async UniTask SetColor(Color color, float duration, Ease ease)
        {
            currentTween = spriteRenderer.DOColor(color, duration).SetEase(ease);
            await currentTween;
        }

        public async UniTask SetBrightness(float brightnessValue, float duration, Ease ease)
        {
            Color targetColor = new Color(brightnessValue, brightnessValue, brightnessValue, spriteRenderer.color.a);
            currentTween = spriteRenderer.DOColor(targetColor, duration).SetEase(ease);
            await currentTween;
        }

        public async UniTask MoveTo(Vector3 position, float duration, Ease ease)
        {
            currentTween = transform.DOMove(position, duration).SetEase(ease);
            await currentTween;
        }

        public async UniTask LocalMoveTo(Vector3 localPosition, float duration, Ease ease)
        {
            currentTween = transform.DOLocalMove(localPosition, duration).SetEase(ease);
            await currentTween;
        }

        public async UniTask RotateTo(Vector3 rotation, float duration, Ease ease)
        {
            currentTween = transform.DORotate(rotation, duration).SetEase(ease);
            await currentTween;
        }

        public async UniTask LocalRotateTo(Vector3 localRotation, float duration, Ease ease)
        {
            currentTween = transform.DOLocalRotate(localRotation, duration).SetEase(ease);
            await currentTween;
        }

        public async UniTask ScaleTo(Vector3 scale, float duration, Ease ease)
        {
            currentTween = transform.DOScale(scale, duration).SetEase(ease);
            await currentTween;
        }

        public async UniTask PunchPosition(Vector3 punch, float duration, int vibrato = 10, float elasticity = 1)
        {
            currentTween = transform.DOPunchPosition(punch, duration, vibrato, elasticity);
            await currentTween;
        }

        public async UniTask PunchRotation(Vector3 punch, float duration, int vibrato = 10, float elasticity = 1)
        {
            currentTween = transform.DOPunchRotation(punch, duration, vibrato, elasticity);
            await currentTween;
        }

        public async UniTask PunchScale(Vector3 punch, float duration, int vibrato = 10, float elasticity = 1)
        {
            currentTween = transform.DOPunchScale(punch, duration, vibrato, elasticity);
            await currentTween;
        }

        public async UniTask ShakePosition(float duration, float strength = 90, int vibrato = 10, bool fadeOut = true)
        {
            currentTween = transform.DOShakePosition(duration, strength, vibrato, 90, fadeOut);
            await currentTween;
        }

        public async UniTask ShakeRotation(float duration, float strength = 90, int vibrato = 10, bool fadeOut = true)
        {
            currentTween = transform.DOShakeRotation(duration, strength, vibrato, 90, fadeOut);
            await currentTween;
        }

        public async UniTask ShakeScale(float duration, float strength = 1, int vibrato = 10, float fadeOut = 0)
        {
            currentTween = transform.DOShakeScale(duration, strength, vibrato, fadeOut);
            await currentTween;
        }

        public async UniTask MoveOnPath(Vector3[] path, float duration, PathType pathType = PathType.CatmullRom, Ease ease = Ease.Linear)
        {
            currentTween = transform.DOPath(path, duration, pathType).SetEase(ease);
            await currentTween;
        }

        public async UniTask LookAtTarget(Vector3 worldPosition, float duration, Ease ease)
        {
            currentTween = transform.DOLookAt(worldPosition, duration).SetEase(ease);
            await currentTween;
        }

        public void SetLoopingMove(Vector3 endValue, float duration, LoopType loopType = LoopType.Yoyo, Ease ease = Ease.Linear)
        {
            currentTween?.Kill();
            currentTween = transform.DOLocalMove(endValue, duration)
                .SetEase(ease)
                .SetLoops(-1, loopType);
        }

        public void SetLoopingRotate(Vector3 endValue, float duration, LoopType loopType = LoopType.Yoyo, Ease ease = Ease.Linear)
        {
            currentTween?.Kill();
            currentTween = transform.DOLocalRotate(endValue, duration)
                .SetEase(ease)
                .SetLoops(-1, loopType);
        }

        public void SetLoopingScale(Vector3 endValue, float duration, LoopType loopType = LoopType.Yoyo, Ease ease = Ease.Linear)
        {
            currentTween?.Kill();
            currentTween = transform.DOScale(endValue, duration)
                .SetEase(ease)
                .SetLoops(-1, loopType);
        }


        public async UniTask Vignette(float time, AnimationBehaviourType animationBehaviour, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await spriteRenderer.DOVignette(animationBehaviour, time).SetEase(ease);
        }

        public async UniTask Vignette(float time, float value, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await spriteRenderer.DOVignette(value, time).SetEase(ease);
        }

        public async UniTask StarWipe(float time, AnimationBehaviourType animationBehaviour, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await spriteRenderer.DOStarWipe(animationBehaviour, time).SetEase(ease);
        }

        public async UniTask StarWipe(float time, float value, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await spriteRenderer.DOStarWipe(value, time).SetEase(ease);
        }

        public async UniTask Dissolve(float time, AnimationBehaviourType animationBehaviour, Ease ease, Texture2D texture = null)
        {
            time = MathfExtensions.ClampTime(time);
            await spriteRenderer.DODissolve(animationBehaviour, time, texture).SetEase(ease);
        }

        public async UniTask ToBlackAndWhite(float time, AnimationBehaviourType animationBehaviour, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await spriteRenderer.DOBlackAndWhite(animationBehaviour, time).SetEase(ease);
        }

        public async UniTask ToBlackAndWhite(float time, float value, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await spriteRenderer.DOBlackAndWhite(value, time).SetEase(ease);
        }

        public async UniTask Celia(float time, AnimationBehaviourType animationBehaviour, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await spriteRenderer.DOCelia(animationBehaviour, time).SetEase(ease);
        }

        public async UniTask Celia(float time, float value, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await spriteRenderer.DOCelia(value, time).SetEase(ease);
        }

        public async UniTask Solid(float time, AnimationBehaviourType animationBehaviour, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await spriteRenderer.DOSolid(animationBehaviour, time).SetEase(ease);
        }

        public async UniTask Solid(float time, float value, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await spriteRenderer.DOSolid(value, time).SetEase(ease);
        }

        public async UniTask Illuminate(float time, AnimationBehaviourType animationBehaviour, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await spriteRenderer.DOIllumination(animationBehaviour, time).SetEase(ease);
        }

        public async UniTask Illuminate(float time, float value, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await spriteRenderer.DOIllumination(value, time).SetEase(ease);
        }

        #endregion
    }
}