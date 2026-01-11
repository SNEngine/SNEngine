using Cysharp.Threading.Tasks;
using SNEngine.InputWindowSystem;
using SNEngine.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Input Window Service")]
    public class InputWindowService : ServiceBase, IShowable, IHidden
    {
        private IInputWindow _inputWindow;

        private const string INPUT_WINDOW_VANILLA_PATH = "UI/InputWindow";

        public override void Initialize()
        {
            var ui = NovelGame.Instance.GetService<UIService>();

            var input = ResourceLoader.LoadCustomOrVanilla<InputWindow>(INPUT_WINDOW_VANILLA_PATH);

            if (input == null)
            {
                return;
            }

            var prefab = Object.Instantiate(input);

            prefab.name = input.name;

            _inputWindow = prefab;

            ui.AddElementToUIContainer(prefab.gameObject);

            prefab.gameObject.SetActive(false);
        }

        public void Show()
        {
            _inputWindow.Show();
        }

        public void Hide()
        {
            _inputWindow.Hide();
        }

        public void SetData(string keyTitle, Sprite icon, string defaultTitle)
        {
            _inputWindow.SetData(keyTitle, icon, defaultTitle);
        }

        public async UniTask<InputWindowResult> WaitInputPlayer()
        {
            return await _inputWindow.WaitInputPlayer();
        }

        public override void ResetState()
        {
            _inputWindow.ResetState();
        }
    }
}