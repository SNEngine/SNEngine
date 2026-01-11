using Cysharp.Threading.Tasks;
using DG.Tweening;
using SNEngine.DialogOnScreenSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SNEngine.Source.SNEngine.MessageSystem
{
    public class MessageOnScreenWindow : MonoBehaviour, IMessageOnScreenWindow
    {
        [SerializeField] private MessageView _bubblePrefab;
        [SerializeField] private Transform _bubbleContainer;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField, Min(0)] private float _scrollDuration = 0.25f;
        [SerializeField] private Ease _ease = Ease.OutQuad;

        private IPrinterNode _node;
        private TMP_FontAsset _currentFont;

        public void SetData(IDialogOnScreenNode node)
        {
            _node = node;
        }

        public void StartOutputDialog()
        {
            if (_node == null)
                return;

            CreateBubble(_node.GetText()).Forget();
        }

        public void ResetState()
        {
            if (_bubbleContainer != null)
            {
                for (int i = _bubbleContainer.childCount - 1; i >= 0; i--)
                {
                    Destroy(_bubbleContainer.GetChild(i).gameObject);
                }
            }
        }

        public void SetFontDialog(TMP_FontAsset font)
        {
            _currentFont = font;

            if (_bubbleContainer == null || font == null)
                return;

            foreach (Transform child in _bubbleContainer)
            {
                var view = child.GetComponent<MessageView>();
                if (view != null && view.Printer != null)
                {
                    view.Printer.SetFontDialog(font);
                }
            }
        }

        private async UniTask CreateBubble(string text)
        {
            var bubble = Instantiate(_bubblePrefab, _bubbleContainer);

            if (_currentFont != null && bubble.Printer != null)
            {
                bubble.Printer.SetFontDialog(_currentFont);
            }

            bubble.ShowMessage(text);

            await WaitBubble(bubble.Printer);

            _node.MarkIsEnd();
            _node = null;
        }

        private async UniTask WaitBubble(MessagePrinterText printer)
        {
            if (printer == null)
                return;

            while (!printer.AllTextWrited)
            {
                await ScrollToBottom();
                await UniTask.Yield();
            }

            await ScrollToBottom();
        }

        private async UniTask ScrollToBottom()
        {
            if (_scrollRect == null)
                return;

            await _scrollRect
                .DONormalizedPos(new Vector2(0, 0), _scrollDuration)
                .SetEase(_ease);
        }
    }
}