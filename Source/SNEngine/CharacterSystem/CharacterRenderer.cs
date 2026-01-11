using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SNEngine.Extensions;
using SNEngine.Animations;
using SNEngine.Repositories;
using SNEngine.Debugging;

namespace SNEngine.CharacterSystem
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CharacterRenderer : MonoBehaviour, ICharacterRenderer
    {
        private SpriteRenderer _main;
        private SpriteRenderer _fx;
        private bool _firstShowDone = false;

        [SerializeField] private float _crossfadeDuration = 0.3f;
        [SerializeField] private Ease _crossfadeEase = Ease.Linear;
        [SerializeField] private Material _blendMaterial;

        private Character _character;
        private Material _defaultMaterial;

        private Material _mainMat;
        private Material _fxMat;

        private const string BlendProperty = "_Blend";
        private const string BlendTexProperty = "_BlendTex";

        public bool SpriteIsSeted => _main.sprite != null;

        private void Awake()
        {
            if (!TryGetComponent(out _main))
                throw new NullReferenceException("main sprite renderer component not found");

            _defaultMaterial = NovelGame.Instance
                .GetRepository<MaterialRepository>()
                .GetMaterial("default");

            _fx = CreateRenderer("FX", 1);
            _fx.gameObject.SetActive(false);

            EnsureMaterials();
        }

        public T AddComponent<T>() where T : Component
        {
            return gameObject.AddComponent<T>();
        }

        private SpriteRenderer CreateRenderer(string name, int orderOffset)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingLayerID = _main.sortingLayerID;
            sr.sortingOrder = _main.sortingOrder + orderOffset;
            sr.material = _main.material;
            sr.color = sr.color.SetAlpha(0);
            return sr;
        }

        private void EnsureMaterials()
        {
            if (_mainMat == null)
            {
                _mainMat = new Material(_main.sharedMaterial ?? _defaultMaterial);
                _main.material = _mainMat;
            }

            if (_fxMat == null)
            {
                _fxMat = new Material(_mainMat);
                _fx.material = _fxMat;
            }
        }

        public void ApplyEffectMaterial(Material template)
        {
            if (template == null) return;

            if (_fxMat != null) Destroy(_fxMat);

            _fxMat = new Material(template);
            _fx.material = _fxMat;
            _fx.sprite = _main.sprite;
            _fx.color = _main.color;
            _fx.gameObject.SetActive(true);
        }

        public void RemoveEffectMaterial()
        {
            if (_fxMat != null)
            {
                Destroy(_fxMat);
                _fxMat = null;
            }
            _fx.gameObject.SetActive(false);
        }

        public void Hide()
        {
            _firstShowDone = false;
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            CalculatePositionForScreen();
        }

        public void SetData(Character data)
        {
            _character = data ?? throw new ArgumentNullException(nameof(data));
        }

        public void ShowWithEmotion(string emotionName = "Default")
        {
            if (_character == null)
            {
                Debug.LogWarning("Character data not set!");
                return;
            }

            Sprite newSprite = _character.GetEmotion(emotionName)?.Sprite;
            if (newSprite == null)
            {
                Debug.LogWarning($"Emotion '{emotionName}' sprite not found!");
                return;
            }

            bool canBlend = _blendMaterial != null && SNEngineRuntimeSettings.Instance.EnableCrossfade;
            bool useBlend = canBlend && _firstShowDone;

            if (_main.sprite == null || _main.sprite != newSprite)
            {
                if (!useBlend)
                {
                    _main.sprite = newSprite;
                    _main.color = Color.white;

                    if (_fx != null)
                    {
                        _fx.sprite = newSprite;
                        _fx.color = Color.white;
                    }

                    Show();
                    _firstShowDone = true;
                    return;
                }

                EnsureMaterials();

                Material activeBlendMat = new Material(_blendMaterial);
                Sprite oldSprite = _main.sprite;
                _main.material = activeBlendMat;
                _main.sprite = newSprite;
                activeBlendMat.SetTexture(BlendTexProperty, oldSprite.texture);
                activeBlendMat.SetFloat(BlendProperty, 1f);

                if (_fx != null && _fx.gameObject.activeSelf)
                    _fx.sprite = newSprite;

                Show();

                DOTween.To(
                    () => activeBlendMat.GetFloat(BlendProperty),
                    v => activeBlendMat.SetFloat(BlendProperty, v),
                    0f,
                    _crossfadeDuration
                ).SetEase(_crossfadeEase).OnComplete(() =>
                {
                    _main.material = _mainMat;
                    Destroy(activeBlendMat);
                });

                _firstShowDone = true;
                return;
            }

            Show();
        }




        public void SetFlip(FlipType flipType)
        {
            _main.Flip(flipType);
            if (_fx != null) _fx.Flip(flipType);
        }

        private void OnGUI()
        {
            CalculatePositionForScreen();
        }

        private void CalculatePositionForScreen()
        {
            if (_main is null || Camera.main is null) return;

            float spriteHeight = _main.bounds.size.y;
            float screenHeight = Camera.main.orthographicSize * 2f;

            float targetY = -screenHeight / 2f + spriteHeight / 2f;

            transform.position = new Vector3(
                transform.position.x,
                targetY,
                transform.position.z
            );
        }

        public void ResetState()
        {
            Vector3 position = transform.position;
            position.x = 0;
            transform.position = position;

            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
            transform.rotation = Quaternion.identity;

            DOTween.Kill(transform);

            _main.sprite = _character.GetEmotion(0).Sprite;
            _main.color = Color.white;
            _main.flipX = false;
            _main.flipY = false;
            _main.material = _defaultMaterial;

            if (_mainMat != null) Destroy(_mainMat);
            if (_fxMat != null) Destroy(_fxMat);

            _mainMat = null;
            _fxMat = null;

            RemoveEffectMaterial();
            SetFlip(FlipType.None);
            Hide();
        }

        private async UniTask ApplySpriteRendererTween(Func<SpriteRenderer, UniTask> tween)
        {
            UniTask mainTask = tween(_main);
            UniTask fxTask = UniTask.CompletedTask;

            if (_fx != null && _fx.gameObject.activeSelf)
                fxTask = tween(_fx);

            await UniTask.WhenAll(mainTask, fxTask);
        }

        #region Animations

        public async UniTask MoveX(float x, float time, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await transform.DOMoveX(x, time).SetEase(ease);
        }

        public async UniTask ShakePosition(float duration, float strength = 90, int vibrato = 10, bool fadeOut = true)
        {
            await transform.DOShakePosition(duration, strength, vibrato, 90f, fadeOut: fadeOut);
        }

        public async UniTask Move(CharacterDirection direction, float time, Ease ease)
        {
            float spriteSizeX = _main.size.x;
            float cameraBorder = Camera.main.aspect * Camera.main.orthographicSize - spriteSizeX / 2;
            float x = direction == CharacterDirection.Left ? -cameraBorder : cameraBorder;
            await transform.DOMoveX(x, time).SetEase(ease);
        }

        public async UniTask Fade(float value, float time, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            value = Mathf.Clamp01(value);
            await ApplySpriteRendererTween(r => r.DOFade(value, time).SetEase(ease).ToUniTask());
        }

        public async UniTask Fade(float time, AnimationBehaviourType behaviour, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            float value = AnimationBehaviourHelper.GetValue(behaviour);
            await ApplySpriteRendererTween(r => r.DOFade(value, time).SetEase(ease).ToUniTask());
        }

        public async UniTask Scale(Vector3 scale, float time, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await transform.DOScale(scale, time).SetEase(ease);
        }

        public async UniTask Rotate(Vector3 angle, float time, Ease ease, RotateMode rotateMode)
        {
            time = MathfExtensions.ClampTime(time);
            await transform.DOLocalRotate(angle, time, rotateMode).SetEase(ease);
        }

        public async UniTask ChangeColor(Color color, float time, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await ApplySpriteRendererTween(r => r.DOColor(color, time).SetEase(ease).ToUniTask());
        }

        public async UniTask Dissolve(float time, AnimationBehaviourType behaviour, Ease ease, Texture2D tex = null)
        {
            time = MathfExtensions.ClampTime(time);
            await ApplySpriteRendererTween(r => r.DODissolve(behaviour, time, tex).SetEase(ease).ToUniTask());
        }

        public async UniTask ToBlackAndWhite(float time, AnimationBehaviourType behaviour, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await ApplySpriteRendererTween(r => r.DOBlackAndWhite(behaviour, time).SetEase(ease).ToUniTask());
        }

        public async UniTask ToBlackAndWhite(float time, float value, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await ApplySpriteRendererTween(r => r.DOBlackAndWhite(value, time).SetEase(ease).ToUniTask());
        }

        public async UniTask Celia(float time, AnimationBehaviourType behaviour, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await ApplySpriteRendererTween(r => r.DOCelia(behaviour, time).SetEase(ease).ToUniTask());
        }

        public async UniTask Celia(float time, float value, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await ApplySpriteRendererTween(r => r.DOCelia(value, time).SetEase(ease).ToUniTask());
        }

        public async UniTask Solid(float time, AnimationBehaviourType behaviour, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await ApplySpriteRendererTween(r => r.DOSolid(behaviour, time).SetEase(ease).ToUniTask());
        }

        public async UniTask Solid(float time, float value, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await ApplySpriteRendererTween(r => r.DOSolid(value, time).SetEase(ease).ToUniTask());
        }

        public async UniTask Illuminate(float time, AnimationBehaviourType behaviour, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await ApplySpriteRendererTween(r => r.DOIllumination(behaviour, time).SetEase(ease).ToUniTask());
        }

        public async UniTask Illuminate(float time, float value, Ease ease)
        {
            time = MathfExtensions.ClampTime(time);
            await ApplySpriteRendererTween(r => r.DOIllumination(value, time).SetEase(ease).ToUniTask());
        }

        public UniTask MoveY(float y, float time, Ease ease)
        {
            NovelGameDebug.LogError("character not supported move my Y");
            return UniTask.CompletedTask;
        }

        #endregion
    }
}