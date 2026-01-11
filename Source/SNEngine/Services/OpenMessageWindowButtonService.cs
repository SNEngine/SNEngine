using SNEngine.Services;
using SNEngine.Source.SNEngine.MessageMenu;
using SNEngine.Utils;
using UnityEngine;

namespace SNEngine.Source.SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Open Message Window Button Service")]
    public class OpenMessageWindowButtonService : ServiceBase
    {
        private IOpenMessageWindowButton _openMessage;
        private const string OPEN_MESSAGE_WINDOW_BUTTON_VANILLA_PATH = "UI/OpenMessageWindowButton";

        public override void Initialize()
        {
            var ui = NovelGame.Instance.GetService<UIService>();

            var input = ResourceLoader.LoadCustomOrVanilla<OpenMessageWindowButton>(OPEN_MESSAGE_WINDOW_BUTTON_VANILLA_PATH);

            if (input == null)
            {
                return;
            }

            var prefab = Object.Instantiate(input);

            prefab.name = input.name;

            _openMessage = prefab;

            ui.AddElementToUIContainer(prefab.gameObject);

            _openMessage.Hide();
        }

        public void Show()
        {
            _openMessage?.Show();
        }

        public void Hide()
        {
            _openMessage?.Hide();
        }

        public override void ResetState()
        {
            _openMessage.ResetState();
        }
    }
}