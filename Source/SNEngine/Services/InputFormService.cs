using SNEngine.InputFormSystem;
using UnityEngine;
using SNEngine.Debugging;
using System.Linq;
using UnityEngine.Events;
using SNEngine.Utils;
using Object = UnityEngine.Object;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Input Form Service")]
    public class InputFormService : ServiceBase, IHidden, ISubmitter, IResetable
    {
        public event UnityAction<string> OnSubmit;

        private IInputForm[] _forms;
        private IInputForm _activeForm;
        
        private const string FORMS_VANILLA_PATH = "UI";

        public override void Initialize()
        {
            var forms = ResourceLoader.LoadAllCustomizable<InputForm>(FORMS_VANILLA_PATH);

            if (forms == null || forms.Length == 0)
            {
                NovelGameDebug.LogError($"No Input Forms were loaded from {FORMS_VANILLA_PATH}. Check folder and prefab components.");
                return;
            }

            var uiService = NovelGame.Instance.GetService<UIService>();
            if (uiService == null)
            {
                NovelGameDebug.LogError("UIService not found! Cannot initialize InputForms.");
                return;
            }

            var validForms = forms.Where(f => f != null).ToList();
            _forms = new IInputForm[validForms.Count];

            for (int i = 0; i < validForms.Count; i++)
            {
                var formInstance = Object.Instantiate(validForms[i]);
                _forms[i] = formInstance;
                uiService.AddElementToUIContainer(formInstance.gameObject);
            }

            NovelGameDebug.Log($"Successfully loaded {_forms.Length} InputForms");
            ResetState();
        }

        public void Show(InputFormType type, string label, bool isTriming)
        {
            var form = _forms.SingleOrDefault(x => x.Type == type);

            if (form is null)
            {
                NovelGameDebug.LogError($"Input form with type {type} not found in {GetType().Name}");
                return;
            }

            form.Label = label;
            form.IsTrimming = isTriming;
            form.Show();

            _activeForm = form;
            _activeForm.OnSubmit += OnSumbitText;
        }

        private void OnSumbitText(string text)
        {
            if (_activeForm != null)
                _activeForm.OnSubmit -= OnSumbitText;

            OnSubmit?.Invoke(text);
        }

        public void Hide()
        {
            _activeForm?.Hide();
            if (_activeForm != null)
                _activeForm.OnSubmit -= OnSumbitText;
            _activeForm = null;
        }

        public override void ResetState()
        {
            if (_forms == null) return;
            foreach (var form in _forms)
            {
                form?.Hide();
            }
        }
    }
}