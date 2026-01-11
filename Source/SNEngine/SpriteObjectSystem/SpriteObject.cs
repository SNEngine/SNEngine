using Cysharp.Threading.Tasks;
using DG.Tweening;
using SNEngine.Animations;
using SNEngine.CharacterSystem;
using SNEngine.Debugging;
using SNEngine.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SNEngine.SpriteObjectSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteObject : MonoBehaviour, ISpriteObject
    {
        private SpriteRenderer _spriteRenderer;
        private Material _defaultMaterial;
        private Tween _currentTween;
        private static readonly Dictionary<int, int> _runtimeSpriteRefCount = new();
        private Color DefaultColor
        {
            get
            {
                Color color = Color.white;
                color.a = 0;
                return color;
            }
        }

        public bool SpriteIsSeted => _spriteRenderer.sprite != null;

        private void Awake()
        {
            if (!TryGetComponent(out _spriteRenderer))
            {
                throw new NullReferenceException("Sprite Renderer component not found on SpriteObject");
            }

            _defaultMaterial = _spriteRenderer.sharedMaterial;
            _spriteRenderer.color = DefaultColor;
        }

        public void SetSprite(Sprite sprite)
        {
            if (_spriteRenderer.sprite != null) TryCleanupRuntimeSprite();

            if (sprite != null && IsRuntimeSprite(sprite))
            {
                int id = sprite.GetInstanceID();
                if (_runtimeSpriteRefCount.ContainsKey(id))
                    _runtimeSpriteRefCount[id]++;
                else
                    _runtimeSpriteRefCount[id] = 1;

                NovelGameDebug.Log($"[SpriteObject] Linked runtime sprite: {sprite.name} | Refs: {_runtimeSpriteRefCount[id]}");
            }

            _spriteRenderer.sprite = sprite;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void TryCleanupRuntimeSprite()
        {
            Sprite sprite = _spriteRenderer.sprite;
            if (sprite == null || !IsRuntimeSprite(sprite)) return;

            int id = sprite.GetInstanceID();

            if (_runtimeSpriteRefCount.ContainsKey(id))
            {
                _runtimeSpriteRefCount[id]--;

                if (_runtimeSpriteRefCount[id] <= 0)
                {
                    NovelGameDebug.Log($"[SpriteObject] Destroying runtime sprite: {sprite.name} (No more refs)");
                    _runtimeSpriteRefCount.Remove(id);

                    _spriteRenderer.sprite = null;

                    if (Application.isPlaying)
                        Destroy(sprite);
                    else
                        DestroyImmediate(sprite);
                }
                else
                {
                    NovelGameDebug.Log($"[SpriteObject] Sprite {sprite.name} still has {_runtimeSpriteRefCount[id]} refs.");
                    _spriteRenderer.sprite = null;
                }
            }
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void ResetState()
        {
            TryCleanupRuntimeSprite();
            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
            transform.rotation = Quaternion.identity;

            _currentTween?.Kill();

            _spriteRenderer.sprite = null;
            _spriteRenderer.color = DefaultColor;
            _spriteRenderer.flipX = false;
            _spriteRenderer.flipY = false;
            _spriteRenderer.material = _defaultMaterial;
            Hide();
        }

        private bool IsRuntimeSprite(Sprite sprite)
        {
            if (sprite == null) return false;
#if UNITY_EDITOR
            return !UnityEditor.EditorUtility.IsPersistent(sprite);
#else
    return true; 
#endif
        }

        public T AddComponent<T>() where T : Component
        {
            return gameObject.AddComponent<T>();
        }

        #region Animations

        public async UniTask Move(Vector3 position, float time, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);

            _currentTween = transform.DOMove(position, time).SetEase(ease);
            await _currentTween;
        }

        public async UniTask MoveX(float x, float time, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);

            _currentTween = transform.DOMoveX(x, time).SetEase(ease);
            await _currentTween;
        }

        public async UniTask MoveY(float y, float time, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);

            _currentTween = transform.DOMoveY(y, time).SetEase(ease);
            await _currentTween;
        }

        public async UniTask Fade(float value, float time, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);

            value = Mathf.Clamp01(value);

            _currentTween = _spriteRenderer.DOFade(value, time).SetEase(ease);
            await _currentTween;
        }

        public async UniTask Fade(float time, AnimationBehaviourType animationBehaviour, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);

            float value = AnimationBehaviourHelper.GetValue(animationBehaviour);

            _currentTween = _spriteRenderer.DOFade(value, time).SetEase(ease);
            await _currentTween;
        }

        public async UniTask Scale(Vector3 scale, float time, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);

            _currentTween = transform.DOScale(scale, time).SetEase(ease);
            await _currentTween;
        }

        public async UniTask Rotate(Vector3 angle, float time, Ease ease, RotateMode rotateMode)
        {
            time = MathfExtensions.ClampTime(time);

            _currentTween = transform.DOLocalRotate(angle, time, rotateMode).SetEase(ease);
            await _currentTween;
        }

        public async UniTask ChangeColor(Color color, float time, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);

            _currentTween = _spriteRenderer.DOColor(color, time).SetEase(ease);
            await _currentTween;
        }


        public async UniTask SetTransperent(float fadeValue, float duration, Ease ease)
        {
            _currentTween = _spriteRenderer.DOFade(fadeValue, duration).SetEase(ease);
            await _currentTween;
        }

        public async UniTask SetColor(Color color, float duration, Ease ease)
        {
            _currentTween = _spriteRenderer.DOColor(color, duration).SetEase(ease);
            await _currentTween;
        }

        public async UniTask SetBrightness(float brightnessValue, float duration, Ease ease)
        {
            Color targetColor = new Color(brightnessValue, brightnessValue, brightnessValue, _spriteRenderer.color.a);
            _currentTween = _spriteRenderer.DOColor(targetColor, duration).SetEase(ease);
            await _currentTween;
        }

        public async UniTask MoveTo(Vector3 position, float duration, Ease ease)
        {
            _currentTween = transform.DOMove(position, duration).SetEase(ease);
            await _currentTween;
        }

        public async UniTask LocalMoveTo(Vector3 localPosition, float duration, Ease ease)
        {
            _currentTween = transform.DOLocalMove(localPosition, duration).SetEase(ease);
            await _currentTween;
        }

        public async UniTask RotateTo(Vector3 rotation, float duration, Ease ease)
        {
            _currentTween = transform.DORotate(rotation, duration).SetEase(ease);
            await _currentTween;
        }

        public async UniTask LocalRotateTo(Vector3 localRotation, float duration, Ease ease)
        {
            _currentTween = transform.DOLocalRotate(localRotation, duration).SetEase(ease);
            await _currentTween;
        }

        public async UniTask ScaleTo(Vector3 scale, float duration, Ease ease)
        {
            _currentTween = transform.DOScale(scale, duration).SetEase(ease);
            await _currentTween;
        }

        public async UniTask PunchPosition(Vector3 punch, float duration, int vibrato = 10, float elasticity = 1)
        {
            _currentTween = transform.DOPunchPosition(punch, duration, vibrato, elasticity);
            await _currentTween;
        }

        public async UniTask PunchRotation(Vector3 punch, float duration, int vibrato = 10, float elasticity = 1)
        {
            _currentTween = transform.DOPunchRotation(punch, duration, vibrato, elasticity);
            await _currentTween;
        }

        public async UniTask PunchScale(Vector3 punch, float duration, int vibrato = 10, float elasticity = 1)
        {
            _currentTween = transform.DOPunchScale(punch, duration, vibrato, elasticity);
            await _currentTween;
        }

        public async UniTask ShakePosition(float duration, float strength = 90, int vibrato = 10, bool fadeOut = true)
        {
            _currentTween = transform.DOShakePosition(duration, strength, vibrato, 90, fadeOut);
            await _currentTween;
        }

        public async UniTask ShakeRotation(float duration, float strength = 90, int vibrato = 10, bool fadeOut = true)
        {
            _currentTween = transform.DOShakeRotation(duration, strength, vibrato, 90, fadeOut);
            await _currentTween;
        }

        public async UniTask ShakeScale(float duration, float strength = 1, int vibrato = 10, float fadeOut = 0)
        {
            _currentTween = transform.DOShakeScale(duration, strength, vibrato, fadeOut);
            await _currentTween;
        }

        public async UniTask MoveOnPath(Vector3[] path, float duration, PathType pathType = PathType.CatmullRom, Ease ease = Ease.Linear)
        {
            _currentTween = transform.DOPath(path, duration, pathType).SetEase(ease);
            await _currentTween;
        }

        public async UniTask LookAtTarget(Vector3 worldPosition, float duration, Ease ease)
        {
            _currentTween = transform.DOLookAt(worldPosition, duration).SetEase(ease);
            await _currentTween;
        }

        public void SetLoopingMove(Vector3 endValue, float duration, LoopType loopType = LoopType.Yoyo, Ease ease = Ease.Linear)
        {
            _currentTween?.Kill();
            _currentTween = transform.DOLocalMove(endValue, duration)
                .SetEase(ease)
                .SetLoops(-1, loopType);
        }

        public void SetLoopingRotate(Vector3 endValue, float duration, LoopType loopType = LoopType.Yoyo, Ease ease = Ease.Linear)
        {
            _currentTween?.Kill();
            _currentTween = transform.DOLocalRotate(endValue, duration)
                .SetEase(ease)
                .SetLoops(-1, loopType);
        }

        public void SetLoopingScale(Vector3 endValue, float duration, LoopType loopType = LoopType.Yoyo, Ease ease = Ease.Linear)
        {
            _currentTween?.Kill();
            _currentTween = transform.DOScale(endValue, duration)
                .SetEase(ease)
                .SetLoops(-1, loopType);
        }

        #endregion
    }
}