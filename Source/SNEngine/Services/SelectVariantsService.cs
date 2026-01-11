using SNEngine.SelectVariantsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Select Variants Service")]
    public class SelectVariantsService : ServiceBase, IShowerVariants, ISelectableVariant
    {
        private IVariantsSelectWindow _window;

        public event Action<int> OnSelect;

        private bool _flagShowInvolvedCharacters = true;

        public override void Initialize()
        {
            var window = Resources.Load<VariantsSelectWindow>("UI/WindowSelecVariants");
            var prefab = Object.Instantiate(window);
            prefab.name = window.name;
            Object.DontDestroyOnLoad(prefab);

            var uiService = NovelGame.Instance.GetService<UIService>();
            uiService.AddElementToUIContainer(prefab.gameObject);

            _window = prefab;
            _window.Hide();
        }

        public void ShowVariants(IEnumerable<string> variants, bool hideCharacters = true, bool hideDialogWindow = true, bool returnCharactersVisible = true, AnimationButtonsType animationType = AnimationButtonsType.None)
        {
            _window.OnSelect -= OnSelectVariant;
            _window.OnSelect += OnSelectVariant;

            _window.ShowVariants(variants, hideCharacters, hideDialogWindow, returnCharactersVisible, animationType);

            _flagShowInvolvedCharacters = returnCharactersVisible;
        }

        public void OnSelectVariant(int index)
        {
            _window.OnSelect -= OnSelectVariant;

            OnSelect?.Invoke(index);

            if (_flagShowInvolvedCharacters)
            {
                var charactersService = NovelGame.Instance.GetService<CharacterService>();
                charactersService.ShowInvolvedCharacters();
            }

            _window.Hide();
        }

        public override void ResetState()
        {
            _window.Hide();
        }
    }
}