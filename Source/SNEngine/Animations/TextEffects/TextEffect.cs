using TMPro;
using UnityEngine;

namespace SNEngine.Animations.TextEffects
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public abstract class TextEffect : MonoBehaviour
    {
        private TextMeshProUGUI textMesh;
        [SerializeField] private PrinterText _printerText;

        protected bool AllTextWrited => _printerText.AllTextWrited;

        protected TextMeshProUGUI Component => textMesh;

        private void Awake()
        {
            textMesh = GetComponent<TextMeshProUGUI>();
        }

        protected virtual void OnEnable()
        {
            if (_printerText != null)
                _printerText.OnTextForceCompleted += HandleComplete;
        }

        protected virtual void OnDisable()
        {
            if (_printerText != null)
                _printerText.OnTextForceCompleted -= HandleComplete;
        }

        private void HandleComplete() => TextForceCompleted(textMesh);
        protected abstract void TextForceCompleted(TextMeshProUGUI textMesh);
        protected void ResetFlagAnimation() => _printerText?.ResetFlagAnimation();
    }
}