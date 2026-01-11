using SNEngine.DialogSystem;
using SNEngine.MainMenuSystem;
using SNEngine.Utils;
using SNEngine.Graphs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Main Menu Service")]
    public class MainMenuService : ServiceBase, IShowable, IHidden
    {
        private IMainMenu _mainMenu;
        private DialogueService _dialogueService;
        private const string MAIN_MENU_VANILLA_PATH = "UI/MainMenu";

        public override void Initialize()
        {
            _dialogueService = NovelGame.Instance.GetService<DialogueService>();
            var ui = NovelGame.Instance.GetService<UIService>();
            var input = ResourceLoader.LoadCustomOrVanilla<MainMenu>(MAIN_MENU_VANILLA_PATH);

            if (input == null) return;

            var prefab = Object.Instantiate(input);
            prefab.name = input.name;
            _mainMenu = prefab;

            ui.AddElementToUIContainer(prefab.gameObject);
            prefab.gameObject.SetActive(false);

            _dialogueService.OnEndDialogue += OnEndDialogue;
        }

        private void OnEndDialogue(IDialogue dialogue)
        {
            if (!dialogue.HasNextDialogueOnExit())
            {
                ResetAllGlobalContainers();
                Show();
            }
        }

        private void ResetAllGlobalContainers()
        {
            var containers = Resources.LoadAll<VariableContainerGraph>("");
            foreach (var container in containers)
            {
                container.ResetState();
            }
        }

        public void Show()
        {
            _mainMenu.Show();
        }

        public void Hide()
        {
            _mainMenu.Hide();
        }
    }
}