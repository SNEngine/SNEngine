using SNEngine.DialogOnScreenSystem;
using SNEngine.DialogSystem;
using SNEngine.Utils;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Dialog On Screen Service")]
    public class DialogueOnScreenService : ServiceBase, IPrinterText, IResetable
    {
        private IDialogOnScreenWindow _window;
        private const string WINDOW_VANILLA_PATH = "UI/DialogOnScreenWindow";


        public override void Initialize()
        {
            DialogOnScreenWindow windowToLoad =
                ResourceLoader.LoadCustomOrVanilla<DialogOnScreenWindow>(WINDOW_VANILLA_PATH);

            if (windowToLoad == null)
            {
                return;
            }

            string prefabName = windowToLoad.name;

            var prefab = Object.Instantiate(windowToLoad);

            prefab.name = prefabName;

            Object.DontDestroyOnLoad(prefab);

            var uiService = NovelGame.Instance.GetService<UIService>();

            uiService.AddElementToUIContainer(prefab.gameObject);

            _window = prefab;

            ResetState();
        }

        public void ResetFont()
        {
            _window.ResetFont();
        }

        public override void ResetState()
        {
            _window.ResetState();
        }

        public void SetFontDialog(TMP_FontAsset font)
        {
            _window.SetFontDialog(font);
        }

        public void ShowDialog(IDialogOnScreenNode dialog)
        {
            _window.SetData(dialog);

            _window.Show();

            _window.StartOutputDialog();
        }
    }
}