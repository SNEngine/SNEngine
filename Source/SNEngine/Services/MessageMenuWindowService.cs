using SNEngine.Services;
using SNEngine.Source.SNEngine.MessageMenu;
using SNEngine.Utils;
using UnityEngine;

namespace SNEngine.Source.SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Message Menu Window Service")]

    public class MessageMenuWindowService : ServiceBase, IShowable, IHidden
    {
        private IMessageMenuWindow _messageMenu;
        private const string MESSAGE_MENU_WINDOW_VANILLA_PATH = "UI/MessageMenuWindow";

        public override void Initialize()
        {
            var ui = NovelGame.Instance.GetService<UIService>();

            var input = ResourceLoader.LoadCustomOrVanilla<MessageMenuWindow>(MESSAGE_MENU_WINDOW_VANILLA_PATH);

            if (input == null)
            {
                return;
            }

            var prefab = Object.Instantiate(input);

            prefab.name = input.name;

            _messageMenu = prefab;

            ui.AddElementToUIContainer(prefab.gameObject);

            prefab.gameObject.SetActive(false);
        }

        public void Show()
        {
            _messageMenu.Show();
        }

        public void Hide()
        {
            _messageMenu.Hide();
        }
    }
}