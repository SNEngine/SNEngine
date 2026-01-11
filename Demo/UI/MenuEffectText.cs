using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace CoreGame.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class MenuEffectText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Colors & Scale")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _hoverColor = Color.yellow;
        [SerializeField] private float _baseScale = 1.0f;
        [SerializeField] private float _hoverScale = 1.1f;

        [Header("Transition Speed")]
        [SerializeField] private bool _enableHoverEffect = true;
        [SerializeField] private float _transitionDuration = 0.08f;

        [Header("Glitch Effect Settings")]
        [SerializeField] private float _glitchInterval = 0.05f;
        [SerializeField] private float _glitchIntensity = 10f;
        [SerializeField] private float _colorGlitchChance = 0.4f;

        private TextMeshProUGUI _textComponent;
        private Vector3 _originalScale;
        private bool _isPointerOver = false;
        private float _glitchTimer;
        private Vector3 _basePosition;

        private JobHandle _glitchJobHandle;
        private NativeArray<float3> _targetOffset;
        private NativeArray<float> _targetScaleMultiplier;
        private NativeArray<int> _glitchColorFlag;

        [BurstCompile]
        private struct GlitchCalculationJob : IJob
        {
            public float GlitchIntensity;
            public float BaseScale;
            public float ColorGlitchChance;
            public uint RandomSeed;

            public NativeArray<float3> TargetOffset;
            public NativeArray<float> TargetScaleMultiplier;
            public NativeArray<int> GlitchColorFlag;

            public void Execute()
            {
                var random = new Random(RandomSeed);

                float xOffset = random.NextFloat(-GlitchIntensity, GlitchIntensity);
                float yOffset = random.NextFloat(-GlitchIntensity, GlitchIntensity);

                TargetOffset[0] = new float3(xOffset, yOffset, 0);

                float scaleOffset = random.NextFloat(-0.01f, 0.01f);
                TargetScaleMultiplier[0] = BaseScale + scaleOffset;

                GlitchColorFlag[0] = random.NextFloat(0f, 1f) < ColorGlitchChance ? 1 : 0;
            }
        }

        private void Awake()
        {
            _textComponent = GetComponent<TextMeshProUGUI>();
            _originalScale = transform.localScale;
            _basePosition = transform.localPosition;
            _textComponent.color = _normalColor;

            _targetOffset = new NativeArray<float3>(1, Allocator.Persistent);
            _targetScaleMultiplier = new NativeArray<float>(1, Allocator.Persistent);
            _glitchColorFlag = new NativeArray<int>(1, Allocator.Persistent);
        }

        private void OnDestroy()
        {
            _glitchJobHandle.Complete();
            if (_targetOffset.IsCreated) _targetOffset.Dispose();
            if (_targetScaleMultiplier.IsCreated) _targetScaleMultiplier.Dispose();
            if (_glitchColorFlag.IsCreated) _glitchColorFlag.Dispose();
        }

        private void Update()
        {
            if (!_isPointerOver || !_enableHoverEffect)
            {
                _glitchTimer += Time.deltaTime;
                if (_glitchTimer >= _glitchInterval)
                {
                    _glitchTimer = 0f;
                    ScheduleGlitchJob();
                }
            }
        }

        private void LateUpdate()
        {
            if (_glitchJobHandle.IsCompleted)
            {
                _glitchJobHandle.Complete();
                if (!_isPointerOver || !_enableHoverEffect)
                {
                    ApplyGlitchJobResults();
                }
            }
        }

        private void ScheduleGlitchJob()
        {
            var job = new GlitchCalculationJob
            {
                GlitchIntensity = _glitchIntensity,
                BaseScale = _baseScale,
                ColorGlitchChance = _colorGlitchChance,
                RandomSeed = (uint)System.Environment.TickCount,

                TargetOffset = _targetOffset,
                TargetScaleMultiplier = _targetScaleMultiplier,
                GlitchColorFlag = _glitchColorFlag
            };

            _glitchJobHandle = job.Schedule();
        }

        private void ApplyGlitchJobResults()
        {
            DOTween.Kill(transform, true);

            Vector3 targetPosition = _basePosition + (Vector3)_targetOffset[0];

            transform.DOLocalMove(targetPosition, 0.01f)
                .SetEase(Ease.Flash)
                .OnComplete(() =>
                {
                    transform.DOLocalMove(_basePosition, 0.01f).SetEase(Ease.Flash);
                });

            if (_glitchColorFlag[0] == 1)
            {
                Color glitchColor = UnityEngine.Random.value > 0.5f ? Color.magenta : Color.cyan;

                _textComponent.DOColor(glitchColor, 0.01f).SetEase(Ease.Linear)
                    .OnComplete(() => _textComponent.DOColor(_normalColor, 0.01f));
            }

            transform.DOScale(_originalScale * _targetScaleMultiplier[0], 0.02f);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_enableHoverEffect) return;

            _glitchJobHandle.Complete();
            _isPointerOver = true;
            DOTween.Kill(transform, true);
            DOTween.Kill(_textComponent, true);

            transform.DOLocalMove(_basePosition, _transitionDuration);

            _textComponent.DOColor(_hoverColor, _transitionDuration);
            transform.DOScale(_originalScale * _hoverScale, _transitionDuration).SetEase(Ease.OutSine);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_enableHoverEffect) return;

            _isPointerOver = false;
            DOTween.Kill(transform, true);
            DOTween.Kill(_textComponent, true);

            _textComponent.DOColor(_normalColor, _transitionDuration);
            transform.DOScale(_originalScale * _baseScale, _transitionDuration).SetEase(Ease.OutSine);

            _glitchTimer = _glitchInterval;
        }

        private void OnEnable()
        {
            if (_textComponent != null)
            {
                _textComponent.color = _normalColor;
            }
            if (_originalScale != Vector3.zero)
            {
                transform.localScale = _originalScale * _baseScale;
            }
            _isPointerOver = false;
        }

        private void OnDisable()
        {
            _glitchJobHandle.Complete();
            DOTween.Kill(transform);
            DOTween.Kill(_textComponent);
            if (_textComponent != null)
            {
                _textComponent.color = _normalColor;
            }
            if (_originalScale != Vector3.zero)
            {
                transform.localScale = _originalScale * _baseScale;
            }
            if (_basePosition != Vector3.zero)
            {
                transform.localPosition = _basePosition;
            }
        }

        private void OnValidate()
        {
            if (!_textComponent)
            {
                _textComponent = GetComponent<TextMeshProUGUI>();
            }
            if (!Application.isPlaying && _textComponent != null)
            {
                _textComponent.color = _normalColor;
                _originalScale = transform.localScale;
                transform.localScale = _originalScale * _baseScale;
            }
        }
    }
}