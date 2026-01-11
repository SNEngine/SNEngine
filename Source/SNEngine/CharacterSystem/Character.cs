using SiphoinUnityHelpers.XNodeExtensions.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using SNEngine.Services;
namespace SNEngine.CharacterSystem
{
    [CreateAssetMenu(menuName = "SNEngine/New Character")]
    public class Character : ScriptableObjectIdentity
    {
        private const string DEFAULT_EMOTION_NAME = "Default";

        [Space]

        [SerializeField] private string _name;

        [Space]

        [SerializeField, TextArea] private string _description;

        [Space]

        [SerializeField] private Color _colorName = Color.white;

        [Space]

        [SerializeField] private Emotion[] _emotions = new Emotion[]
        {
            new Emotion(DEFAULT_EMOTION_NAME)
        };


        public string Description => _description;
        public Color ColorName => _colorName;

        public IEnumerable<Emotion> Emotions => _emotions;

        public string OriginalName => _name;

        public string GetName ()
        {
            LanguageService languageService = NovelGame.Instance.GetService<LanguageService>();
            return languageService.TransliteNameCharacter(this);
        }

        public Emotion GetEmotion (string name = DEFAULT_EMOTION_NAME)
        {
            return _emotions.SingleOrDefault(x  => x.Name == name);
        }

        public Emotion GetEmotion(int index = 0)
        {
            try
            {
                return _emotions[index];
            }
            catch
            {

                throw new NullReferenceException($"emotion with index {index} not found");
            }
        }

        public string GetNameWithColor ()
        {
            return _colorName.ToColorTag(GetName());
        }

#if UNITY_EDITOR
        public void Editor_SetName(string newName)
        {
            _name = newName;
        }

        public void Editor_SetDescription(string newDescription)
        {
            _description = newDescription;
        }

        public void Editor_SetColorName(Color newColor)
        {
            _colorName = newColor;
        }
#endif
    }
}