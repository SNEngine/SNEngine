using Cysharp.Threading.Tasks;
using SNEngine.Debugging;
using SNEngine.IO;
using SNEngine.Polling;
using SNEngine.Services;
using SNEngine.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SNEngine.Localization.UI
{
    public class LanguageListView : MonoBehaviour, ILanguageListView
    {
        [SerializeField] private RectTransform _containerLanguages;
        [SerializeField] private LanguageTooltipView _tooltip;
        private PoolMono<LanguageSelectView> _pool;
        private Dictionary<string, Sprite> _cachedFlagSprites = new();
        private List<Texture2D> _texturesFlags = new();

        private async void OnEnable()
        {
            foreach (var flag in _cachedFlagSprites.Values)
            {
                Destroy(flag);
            }
            _cachedFlagSprites.Clear();

            foreach (var texture in _texturesFlags)
            {
                Destroy(texture);
            }
            _texturesFlags.Clear();

            if (_pool is null)
            {
                var prefab = ResourceLoader.LoadCustomOrVanilla<LanguageSelectView>("UI/selectLanguageView");
                _pool = new(prefab, _containerLanguages, 9, true);
            }
            for (int i = 0; i < _containerLanguages.childCount; i++)
            {
                var child = _containerLanguages.GetChild(i).gameObject;
                child.SetActive(false);
            }

            var languageService = NovelGame.Instance.GetService<LanguageService>();
            var data = await languageService.LoadAvailableLanguagesPreloadDataAsync();

            foreach (var languageData in data)
            {
                var languageCode = languageData.Key;
                var langData = languageData.Value;

                if (!_cachedFlagSprites.TryGetValue(languageCode, out var flagSprite))
                {
                    flagSprite = await LoadFlagTextureAsync(langData.PathFlag);

                    if (flagSprite != null)
                    {
                        _cachedFlagSprites.Add(languageCode, flagSprite);
                    }
                }

                if (flagSprite == null) continue;

                var view = _pool.GetFreeElement();
                view.gameObject.SetActive(true);

                view.SetData(langData.MetaData, flagSprite, languageCode);
                view.OnHover -= OnHoverLanguage;
                view.OnSelect -= OnLanguageSelected;
                view.OnExitPointer -= OnExitHoverLanguage;
                view.OnSelect += OnLanguageSelected;
                view.OnHover += OnHoverLanguage;
                view.OnExitPointer += OnExitHoverLanguage;
            }
        }

        private void OnExitHoverLanguage(string code)
        {
            _tooltip.gameObject.SetActive(false);
        }

        private async void OnHoverLanguage(string code)
        {
            var languageService = NovelGame.Instance.GetService<LanguageService>();
            var languages = await languageService.LoadAvailableLanguagesPreloadDataAsync();
            var targetLanguage = languages.FirstOrDefault(x => x.Key == code);

            if (targetLanguage.Value != null)
            {
                if (_cachedFlagSprites.TryGetValue(code, out var flagSprite))
                {
                    _tooltip.SetData(targetLanguage.Value, flagSprite);
                    _tooltip.gameObject.SetActive(true);
                }
            }
        }

        private void OnDisable()
        {
            foreach (var flag in _cachedFlagSprites.Values)
            {
                Destroy(flag);
            }
            _cachedFlagSprites.Clear();

            foreach (var texture in _texturesFlags)
            {
                Destroy(texture);
            }
            _texturesFlags.Clear();
        }

        private async UniTask<Sprite> LoadFlagTextureAsync(string absolutePath)
        {
            try
            {
                byte[] flagBytes = await NovelFile.ReadAllBytesAsync(absolutePath);

                Texture2D flagTexture = new Texture2D(2, 2);
                flagTexture.LoadImage(flagBytes);
                flagTexture.name = Path.GetFileName(absolutePath);
                _texturesFlags.Add(flagTexture);

                Sprite sprite = Sprite.Create(
                    flagTexture,
                    new Rect(0, 0, flagTexture.width, flagTexture.height),
                    Vector2.one * 0.5f,
                    100.0f
                );

                return sprite;
            }
            catch (Exception ex)
            {
                NovelGameDebug.LogError($"Failed to load flag texture from {absolutePath}: {ex.Message}");
                return null;
            }
        }

        private void OnLanguageSelected(string code)
        {
            var languageService = NovelGame.Instance.GetService<LanguageService>();
            languageService.LoadLanguage(code).Forget();
            var userDataService = NovelGame.Instance.GetService<UserDataService>();
            userDataService.Data.CurrentLanguage = code;
            userDataService.SaveAsync().Forget();
        }

        public void Hide()
        {
            _tooltip.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        public void Show()
        {
            _tooltip.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
    }
}