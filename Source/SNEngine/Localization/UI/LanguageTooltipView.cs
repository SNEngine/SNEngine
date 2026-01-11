using SNEngine.Extensions;
using SNEngine.Localization.Models;
using SNEngine.Services;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SNEngine.Localization.UI
{
    public class LanguageTooltipView : MonoBehaviour
    {
        [SerializeField] private Image _iconLanguage;
        [SerializeField] private TextMeshProUGUI _nameLanguage;
        [SerializeField] private TextMeshProUGUI _info;

        private static readonly string[] keys = new string[]
        {
            nameof(LanguageMetaData.Version).ToLower(),
            nameof(LanguageMetaData.Author).ToLower(),
        };

        public void SetData (PreloadLanguageData data, Sprite icon)
        {
           _nameLanguage.text = data.MetaData.NameLanguage;
           _iconLanguage.sprite = icon;
           _iconLanguage.SetAdaptiveSize();
            var languageService = NovelGame.Instance.GetService<LanguageService>();
            StringBuilder infoContainer = new();
            infoContainer.AppendLine($"{languageService.TransliteUI(keys[1])}: {data.MetaData.Author}");
            infoContainer.AppendLine($"{languageService.TransliteUI(keys[0])}: {data.MetaData.Version}");
            _info.text = infoContainer.ToString();




        }
    }
}