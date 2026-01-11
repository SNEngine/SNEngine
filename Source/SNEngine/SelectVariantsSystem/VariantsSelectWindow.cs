using SNEngine.Polling;
using UnityEngine;
using System;
using System.Collections.Generic;
using SNEngine.Debugging;
using UnityEngine.UI;
using SNEngine.Services;
using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using System.Linq;

namespace SNEngine.SelectVariantsSystem
{
    [RequireComponent(typeof(RectTransform))]
    public class VariantsSelectWindow : MonoBehaviour, IVariantsSelectWindow
    {
        private int RESIZE_IF_BUTTONS_BETWEEN = 5;
        private bool _returnCharactersVisible;
        private PoolMono<VariantButton> _pool;
        private RectTransform _rectTransform;
        private RectTransform _rectTransformScroll;
        private Vector3 _defaultSizeDeltaScrool;
        private Vector3 _defaultPositionScroll;

        [SerializeField, ReadOnly(ReadOnlyMode.OnEditor)] private VariantButton _buttonPrefab;
        [SerializeField, ReadOnly(ReadOnlyMode.OnEditor)] private VariantButton _buttonCustomPrefab;
        [SerializeField, ReadOnly(ReadOnlyMode.OnEditor)] private ScrollRect _scrollRect;
        [SerializeField, ReadOnly(ReadOnlyMode.OnEditor)] private Transform _container;

        [Space]
        [SerializeField, Min(2)] private int _buttonsCreatedOnStart = 5;

        public event Action<int> OnSelect;

        private void Awake()
        {
            if (!_container) _container = transform;

            if (!TryGetComponent(out _rectTransform))
                throw new NullReferenceException("rect transform null");

            if (!_scrollRect.TryGetComponent(out _rectTransformScroll))
                throw new NullReferenceException("scroll rect rect transform component not found");

            var prefab = _buttonCustomPrefab ?? _buttonPrefab;
            _pool = new PoolMono<VariantButton>(prefab, _container, _buttonsCreatedOnStart, true);

            _defaultSizeDeltaScrool = _rectTransformScroll.sizeDelta;
            _defaultPositionScroll = _rectTransformScroll.localPosition;

            HideButtons();
        }

        public void Hide()
        {
            if (_pool != null)
            {
                _pool.HideAllElements();
            }

            gameObject.SetActive(false);

            if (_rectTransformScroll != null)
            {
                _rectTransformScroll.anchoredPosition = _defaultPositionScroll;
                _rectTransformScroll.sizeDelta = _defaultSizeDeltaScrool;
            }
        }

        private void HideButtons()
        {
            if (_pool == null) return;
            var buttons = _pool.Objects;
            foreach (var button in buttons)
            {
                button.OnSelect -= OnSelectVariant;
                button.Hide();
            }
        }

        public void SetData(IEnumerable<string> data, AnimationButtonsType animationType)
        {
            var strings = data.ToArray();
            foreach (var item in strings)
            {
                var button = _pool.GetFreeElement();

                button.OnSelect -= OnSelectVariant;
                button.OnSelect += OnSelectVariant;

                button.SetData(item, animationType);
                button.Show();
            }

            UpdateLayout();
        }

        private void UpdateLayout()
        {
            int activeCount = 0;
            foreach (var obj in _pool.Objects)
            {
                if (obj.gameObject.activeSelf) activeCount++;
            }

            if (activeCount >= RESIZE_IF_BUTTONS_BETWEEN)
            {
                _rectTransformScroll.localPosition = Vector3.zero;
                _rectTransformScroll.sizeDelta = _rectTransform.sizeDelta;
            }
            else
            {
                _rectTransformScroll.localPosition = _defaultPositionScroll;
                _rectTransformScroll.sizeDelta = _defaultSizeDeltaScrool;
            }
        }

        private void OnSelectVariant(int index)
        {
            OnSelect?.Invoke(index);

            if (_returnCharactersVisible)
            {
                var charactersService = NovelGame.Instance.GetService<CharacterService>();
                charactersService.ShowInvolvedCharacters();
            }

            Hide();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void ShowVariants(IEnumerable<string> variants, bool hideCharacters = true, bool hideDialogWindow = true, bool returnCharactersVisible = true, AnimationButtonsType animationType = AnimationButtonsType.None)
        {
            if (hideDialogWindow)
            {
                var dialogUIService = NovelGame.Instance.GetService<DialogueUIService>();
                dialogUIService.HideDialog();
            }

            if (hideCharacters)
            {
                var charactersService = NovelGame.Instance.GetService<CharacterService>();
                charactersService.HideInvolvedCharacters();
            }

            _returnCharactersVisible = returnCharactersVisible;

            SetData(variants, animationType);
            Show();
        }

        public void ResetState()
        {
            Hide();
        }

        public void ClearBeforeShow()
        {
            HideButtons();
        }
    }
}