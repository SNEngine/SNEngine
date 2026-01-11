using SNEngine.DialogSystem;
using SNEngine.Localization.UI;
using SNEngine.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Language List View Service")]
    public class LanguageListViewService :  ServiceBase, IShowable, IHidden
    {
        private ILanguageListView _view;


        public override void Initialize()
        {
            var ui = NovelGame.Instance.GetService<UIService>();

            var input =  ResourceLoader.LoadCustomOrVanilla<LanguageListView>("UI/selectLanguageWindow");

            var prefab = Object.Instantiate(input);

            prefab.name = input.name;

            _view = prefab;

            ui.AddElementToUIContainer(prefab.gameObject);

            _view.Hide();
        }

        public void Show()
        {
            _view.Show();
        }


        public void Hide()
        {
            _view.Hide();
        }
    }
}
