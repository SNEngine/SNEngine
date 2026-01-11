using SNEngine.PauseWindowSystem;
using SNEngine.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Pause Window Service")]
    public class PauseWindowService : ServiceBase, IShowable, IHidden
    {
        private IPauseWindow _pauseWindow;

        private const string PAUSE_WINDOW_VANILLA_PATH = "UI/PauseWindow";

        public override void Initialize()
        {
            var ui = NovelGame.Instance.GetService<UIService>();

            var input = ResourceLoader.LoadCustomOrVanilla<PauseWindow>(PAUSE_WINDOW_VANILLA_PATH);

            if (input == null)
            {
                return;
            }

            var prefab = Object.Instantiate(input);

            prefab.name = input.name;

            _pauseWindow = prefab;

            ui.AddElementToUIContainer(prefab.gameObject);

            prefab.gameObject.SetActive(false);
        }

        public void Show()
        {
            _pauseWindow.Show();
        }

        public void Hide()
        {
            _pauseWindow.Hide();
        }
    }
}