using SNEngine.PauseWindowSystem;
using SNEngine.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Open Pause Window Button Service")]
    public class OpenPauseWindowButtonService : ServiceBase, IShowable, IHidden
    {
        private IOpenPauseWindowButton _openPauseWindowButton;

        private const string OPEN_PAUSE_WINDOW_BUTTON_VANILLA_PATH = "UI/OpenPauseWindowButton";

        public override void Initialize()
        {
            var ui = NovelGame.Instance.GetService<UIService>();

            var input = ResourceLoader.LoadCustomOrVanilla<OpenPauseWindowButton>(OPEN_PAUSE_WINDOW_BUTTON_VANILLA_PATH);

            if (input == null)
            {
                return;
            }

            var prefab = Object.Instantiate(input);

            prefab.name = input.name;

            _openPauseWindowButton = prefab;

            ui.AddElementToUIContainer(prefab.gameObject);

            _openPauseWindowButton.Hide();
        }

        public void Show()
        {
            _openPauseWindowButton.Show();
        }

        public void Hide()
        {
            _openPauseWindowButton.Hide();
        }

        public override void ResetState()
        {
            _openPauseWindowButton.ResetState();
        }
    }
}