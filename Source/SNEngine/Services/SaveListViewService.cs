using SNEngine.Services;
using SNEngine.Utils;
using UnityEngine;
using Object = UnityEngine.Object;
using SNEngine.SaveSystem.UI;

namespace SNEngine.SaveSystem
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Save List View Service")]
    public class SaveListViewService : ServiceBase, IShowable, IHidden
    {
        private SaveListView _view;

        public override void Initialize()
        {
            var ui = NovelGame.Instance.GetService<UIService>();

            var input = ResourceLoader.LoadCustomOrVanilla<SaveListView>("UI/selectSaveWindow");

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

        public async void RefreshList()
        {
            _view.RefreshList();
        }
    }
}