using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using SNEngine.Services;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace SNEngine.Localization
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UILocalizationText : MonoBehaviour
    {
        [SerializeField] private string _key;
        [SerializeField, ReadOnly(ReadOnlyMode.Always)] private TextMeshProUGUI _component;
        [SerializeField] private bool _autoLocalize = true;

        public bool NotCanTranslite { get; private set; }

        private LanguageService LanguageService => NovelGame.Instance.GetService<LanguageService>();
        private object[] _formatArgs;
        private string _fallbackValue;
        private string _initialEditorText;

        private void Awake()
        {
            if (_component != null)
            {
                _initialEditorText = _component.text;
            }
        }

        private void OnEnable()
        {
            LanguageService.OnLanguageLoaded += OnLanguageLoaded;
            if (_autoLocalize)
            {
                Translite();
            }
        }

        private void OnDisable()
        {
            LanguageService.OnLanguageLoaded -= OnLanguageLoaded;
        }

        private void OnLanguageLoaded(string languageCode)
        {
            Translite();
        }

        public void Translite()
        {
            NotCanTranslite = !LanguageService.LanguageIsLoaded;

            if (!string.IsNullOrWhiteSpace(_key) && _key.StartsWith("%"))
            {
                _component.text = LocalizationConstants.GetValue(_key);
                return;
            }

            string translated = LanguageService.TransliteUI(_key);
            string result;

            if (translated != _key)
            {
                result = translated;
            }
            else if (!string.IsNullOrEmpty(_fallbackValue))
            {
                result = _fallbackValue;
            }
            else
            {
                result = _initialEditorText;
            }

            _component.text = GetFormattedString(result);
        }

        public void ChangeKey(string key, string fallback = "")
        {
            _key = key;
            _fallbackValue = fallback;
            _formatArgs = ExtractFormatArguments(fallback);
            Translite();
        }

        public void ChangeKeyWithFormat(string key, params object[] args)
        {
            _key = key;
            _formatArgs = args;
            _fallbackValue = "";
            Translite();
        }

        private string GetFormattedString(string template)
        {
            if (string.IsNullOrEmpty(template)) return template;

            if (_formatArgs != null && _formatArgs.Length > 0 && template.Contains("{") && template.Contains("}"))
            {
                try
                {
                    return string.Format(template, _formatArgs);
                }
                catch
                {
                    return template;
                }
            }
            return template;
        }

        private object[] ExtractFormatArguments(string text)
        {
            if (string.IsNullOrEmpty(text) || !text.Contains("'")) return null;

            var args = new List<object>();
            int start = 0;
            while (start < text.Length)
            {
                int quoteStart = text.IndexOf('\'', start);
                if (quoteStart == -1) break;

                int quoteEnd = text.IndexOf('\'', quoteStart + 1);
                if (quoteEnd == -1) break;

                string argValue = text.Substring(quoteStart + 1, quoteEnd - quoteStart - 1);
                args.Add(argValue);

                start = quoteEnd + 1;
            }
            return args.Count > 0 ? args.ToArray() : null;
        }

        private void OnValidate()
        {
            if (!_component) _component = GetComponent<TextMeshProUGUI>();
        }
    }
}