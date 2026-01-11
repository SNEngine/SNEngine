using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using SNEngine.Debugging;
using SNEngine.Services;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SNEngine.Localization
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class SelectLanguageButton : MonoBehaviour
    {
        [SerializeField, ReadOnly(ReadOnlyMode.Always)] private Button _button;
        [SerializeField, ReadOnly(ReadOnlyMode.Always)] private Image _image;
        private LanguageService LanguageService => NovelGame.Instance.GetService<LanguageService>();
        private Sprite _currentFlagSprite;

        private void Awake()
        {
            _button.enabled = false;
            _image.enabled = false;
            _button.onClick.AddListener(OpenLanguageSelectWindow);
        }

        private void OpenLanguageSelectWindow()
        {
            NovelGame.Instance.GetService<LanguageListViewService>().Show();
        }

        private void OnEnable()
        {
            LanguageService.OnLanguageLoaded += OnLanguageLoaded;

            if (LanguageService.LanguageIsLoaded)
            {
                OnLanguageLoaded(LanguageService.CurrentLanguageCode);
            }
        }

        private void OnDisable()
        {
            LanguageService.OnLanguageLoaded -= OnLanguageLoaded;
        }

        private void OnLanguageLoaded(string codeLanguage)
        {
            if (_currentFlagSprite != null)
            {
                Destroy(_currentFlagSprite);
                _currentFlagSprite = null;
            }

            Texture2D flag = LanguageService.Flag;

            if (flag != null && _image != null)
            {
                Sprite newFlagSprite = Sprite.Create(
                    flag,
                    new Rect(0.0f, 0.0f, flag.width, flag.height),
                    Vector2.one * 0.5f,
                    100.0f
                );

                _image.sprite = newFlagSprite;
                _currentFlagSprite = newFlagSprite;
                _image.enabled = true;
                _button.enabled = true;
            }
            else
            {
                NovelGameDebug.LogWarning($"Flag or Image component is missing for language {codeLanguage}.");
                _image.sprite = null;
            }
        }

        private void OnDestroy()
        {
            if (_currentFlagSprite != null)
            {
                Destroy(_currentFlagSprite);
            }
        }

        private void OnValidate()
        {
            if (!_image)
            {
                _image = GetComponent<Image>();
            }

            if (!_button)
            {
                _button = GetComponent<Button>();
            }
        }
    }
}