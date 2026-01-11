using System.Linq;
using SNEngine.DialogOnScreenSystem;
using SNEngine.DialogSystem;
using SNEngine.Graphs;
using SNEngine.Services;
using SNEngine.Source.SNEngine.MessageSystem;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Source.SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/MessageService")]
    public class MessageService : ServiceBase, IResetable
    {
        private DialogueService _dialogueService;
        private IMessageOnScreenWindow _onScreenWindow;
        private const string WINDOW_VANILLA_PATH = "UI/MessageOnScreenWindow";

        private IDialogue _currentDialogue;

        public override void Initialize()
        {
            _dialogueService = NovelGame.Instance.GetService<DialogueService>();
            _dialogueService.OnStartDialogue += StartDialogHandler;
            _dialogueService.OnEndDialogue += OnEndDialogHandler;

            var prefabGO = Resources.Load<GameObject>(WINDOW_VANILLA_PATH);
            if (prefabGO == null)
            {
                Debug.LogError($"MessageOnScreenWindow prefab not found at Resources/{WINDOW_VANILLA_PATH}");
                return;
            }

            var instance = Object.Instantiate(prefabGO);
            instance.name = prefabGO.name;

            var windowComponent = instance.GetComponent<MessageOnScreenWindow>();
            if (windowComponent == null)
            {
                Debug.LogError("MessageOnScreenWindow component missing on prefab!");
                return;
            }

            _onScreenWindow = windowComponent;

            DontDestroyOnLoad(instance);

            NovelGame.Instance.GetService<UIService>()
                .AddElementToUIContainer(instance);

            instance.SetActive(false);

            ResetState();
        }

        private void StartDialogHandler(IDialogue dialogue)
        {
            _currentDialogue = dialogue;

            var graph = _currentDialogue as DialogueGraph;
            if (graph == null) return;

            var printerNodes = graph.AllNodes.Values.OfType<PrinterTextNode>();

            foreach (var node in printerNodes)
            {
                node.OnMessage += OnMessageSubscribe;
            }
        }

        private void OnEndDialogHandler(IDialogue dialogue)
        {
            var graph = _currentDialogue as DialogueGraph;
            if (graph == null) return;

            var printerNodes = graph.AllNodes.Values.OfType<PrinterTextNode>();

            foreach (var node in printerNodes)
            {
                node.OnMessage -= OnMessageSubscribe;
            }
        }

        private void OnMessageSubscribe(IPrinterNode node)
        {
            var dialogNode = node as IDialogOnScreenNode;
            if (dialogNode == null) return;
        }

        public override void ResetState()
        {
            _onScreenWindow?.ResetState();
        }

        public void SetFontDialog(TMP_FontAsset font)
        {
            _onScreenWindow?.SetFontDialog(font);
        }
        
        public void ShowMessage(IDialogOnScreenNode dialog)
        {
            if (_onScreenWindow == null)
            {
                Debug.LogError("MessageOnScreenWindow is not initialized!");
                return;
            }

            var windowGO = ((MessageOnScreenWindow)_onScreenWindow).gameObject;
            windowGO.SetActive(true);

            _onScreenWindow.SetData(dialog);
            _onScreenWindow.StartOutputDialog();
        }
    }
}