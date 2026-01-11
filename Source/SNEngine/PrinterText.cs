using Cysharp.Threading.Tasks;
using SNEngine.Animations.TextEffects;
using SNEngine.Debugging;
using SNEngine.DialogSystem;
using SNEngine.InputSystem;
using SNEngine.Services;
using System;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SNEngine
{
    public abstract class PrinterText : MonoBehaviour, IPrinterText, IShowable, IHidden
    {
        protected CancellationTokenSource _cancellationTokenSource;

        public event Action OnWriteSymbol;
        public event Action OnTextForceCompleted;

        private string _currentText;

        [SerializeField, Min(0)] private float _speedWriting = 0.3f;
        [SerializeField] private TextMeshProUGUI _textMessage;

        public TextMeshProUGUI TextMessage
        {
            get { return _textMessage; }
            set => _textMessage = value;
        }

        public IInputSystem InputSystem
        {
            get { return _inputSystem; }
            set => _inputSystem = value;
        }

        private TMP_FontAsset _defaultFontTextDialog;
        private IInputSystem _inputSystem;
        private bool _hasTextEffects;

        public bool AllTextWrited => _textMessage.text == _currentText;
        public string CurrentText => _currentText;
        public float SpeedWriting => _speedWriting;

        protected virtual void Awake()
        {
            _defaultFontTextDialog = _textMessage.font;
            _inputSystem = NovelGame.Instance.GetService<InputService>();
        }

        public void DisableTextPrinting()
        {
            TextMessage = null;
        }

        public virtual void Hide() => gameObject.SetActive(false);

        public virtual void Show()
        {
            _hasTextEffects = GetComponentInChildren<TextEffect>() != null;
            gameObject.SetActive(true);
        }

        public void InvokeTextForceCompleted()
        {
            OnTextForceCompleted?.Invoke();
        }

        public virtual void Print(string message)
        {
            Show();
            StartOutputDialog(message);
        }

#if UNITY_STANDALONE || UNITY_WEBGL
        protected virtual void OnPress(KeyCode key)
        {
            if (_cancellationTokenSource != null)
            {
                if (key == KeyCode.Space || key == KeyCode.Mouse0)
                    EndWrite();
            }
        }
#endif

#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
        protected virtual void OnButtonPress(KeyCode key)
        {
            if (_cancellationTokenSource != null)
            {
                if (key == KeyCode.JoystickButton0)
                    EndWrite();
            }
        }
#endif

        protected virtual void StartOutputDialog(string message)
        {
#if UNITY_STANDALONE || UNITY_WEBGL
            _inputSystem.AddListener(OnPress, StandaloneInputEventType.KeyDown, false);
#endif

#if UNITY_ANDROID || UNITY_IOS
            _inputSystem.AddListener(OnTapScreen, MobileInputEventType.TouchBegan, false);
#endif

#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
            _inputSystem.AddListener(OnButtonPress, GamepadButtonEventType.ButtonDown, false);
#endif

            if (_hasTextEffects)
                WriteInstantWithEffects(message).Forget();
            else
                Writing(message).Forget();
        }

#if UNITY_ANDROID || UNITY_IOS
        protected virtual void OnTapScreen(Touch touch)
        {
            if (_cancellationTokenSource != null)
                EndWrite();
        }
#endif

        protected virtual void EndWrite()
        {
            if (_hasTextEffects)
            {
                _hasTextEffects = false;
                ShowAllText();
                return;
            }

            if (AllTextWrited)
            {
                End();
                return;
            }

            ShowAllText();
        }

        private void ShowAllText()
        {
            _cancellationTokenSource?.Cancel();
            _textMessage.text = _currentText;
            OnTextForceCompleted?.Invoke();
        }

        protected virtual void End()
        {
            _cancellationTokenSource = null;

#if UNITY_STANDALONE || UNITY_WEBGL
            _inputSystem.RemoveListener(OnPress, StandaloneInputEventType.KeyDown);
#endif

#if UNITY_ANDROID || UNITY_IOS
            _inputSystem.RemoveListener(OnTapScreen, MobileInputEventType.TouchBegan);
#endif

#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
            _inputSystem.RemoveListener(OnButtonPress, GamepadButtonEventType.ButtonDown);
#endif

            Hide();
        }

        protected virtual async UniTask Writing(string message)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            _currentText = message;
            var sb = new StringBuilder();

            for (int i = 0; i < message.Length; i++)
            {
                if (token.IsCancellationRequested)
                    break;

                sb.Append(message[i]);
                _textMessage.text = sb.ToString();

                _textMessage.ForceMeshUpdate();
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

                OnWriteSymbol?.Invoke();

                await UniTask.Delay(TimeSpan.FromSeconds(_speedWriting), cancellationToken: token);
            }
        }

        private async UniTask WriteInstantWithEffects(string message)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            _currentText = message;

            _textMessage.text = message;
            _textMessage.ForceMeshUpdate();

            for (int i = 0; i < message.Length; i++)
            {
                if (token.IsCancellationRequested)
                    break;

                OnWriteSymbol?.Invoke();
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            }
        }

        public void SetFontDialog(TMP_FontAsset font)
        {
            if (font == null)
            {
                NovelGameDebug.LogError($"font for text dialog is null");
                return;
            }

            _textMessage.font = font;
        }

        public virtual void ResetFont() => _textMessage.font = _defaultFontTextDialog;

        public virtual void ResetState()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = null;
            _textMessage.text = string.Empty;
            _currentText = string.Empty;
            ResetFont();
            Hide();
        }

        public void ResetFlagAnimation()
        {
            _hasTextEffects = false;
        }
    }
}