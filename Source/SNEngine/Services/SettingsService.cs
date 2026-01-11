using SNEngine.Services;
using SNEngine.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Audio.UI.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Settings Service")]
    public class SettingsService : ServiceBase
    {
        private ISettingsWindow _settingsWindow;

        private const string SETTINGS_WINDOW_VANILLA_PATH = "UI/SettingsWindow";

        public override void Initialize()
        {
            var ui = NovelGame.Instance.GetService<UIService>();

            var input = ResourceLoader.LoadCustomOrVanilla<SettingsWindow>(SETTINGS_WINDOW_VANILLA_PATH);

            if (input == null)
            {
                return;
            }

            var prefab = Object.Instantiate(input);

            prefab.name = input.name;

            _settingsWindow = prefab;

            ui.AddElementToUIContainer(prefab.gameObject);

            prefab.gameObject.SetActive(false);
        }

        public void Show()
        {
            if (_settingsWindow is MonoBehaviour window)
            {
                window.gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            if (_settingsWindow is MonoBehaviour window)
            {
                window.gameObject.SetActive(false);
            }
        }
    }
}