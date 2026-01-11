using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Debugging;
using SNEngine.Localization;
using SNEngine.SaveSystem;
using SNEngine.Services;
using UnityEngine;
using XNode;

namespace SNEngine.InputFormSystem
{
    public class ShowInputFormNode : AsyncNode, ILocalizationNode, ISaveProgressNode
    {
        [SerializeField] private string _label = "Input Value";

        [Space]

        [Input(connectionType = ConnectionType.Override), SerializeField] private bool _trimming = false;

        private InputFormType _type;

        [Space]

        [Output(ShowBackingValue.Never), SerializeField] private string _output;

        private InputFormService _service;
        private string _currentLabel;
        private bool _edited;

        public override void Execute()
        {
            if (_edited)
            {
                return;
            }
            if (string.IsNullOrEmpty(_currentLabel))
            {
                _currentLabel = _label;
            }
            base.Execute();

            bool isTrimming = _trimming;

            var input = GetInputPort(nameof(_trimming));

            if (input.Connection != null)
            {
                isTrimming = GetDataFromPort<bool>(nameof(_trimming));
            }

            _service = NovelGame.Instance.GetService<InputFormService>();

            _service.Show(_type, _currentLabel, isTrimming);

            _service.OnSubmit += OnSubmit;
        }

        private void OnSubmit(string text)
        {
            _service.OnSubmit -= OnSubmit;
            _service.Hide();

            _output = text;

            StopTask();
        }
        #region Localization
        public object GetOriginalValue()
        {
            return _label;
        }

        public void SetValue(object value)
        {
            if (value is string == false)
            {
                NovelGameDebug.LogError($"Error SetValue for node {GetType().Name} GUID {GUID} type not a String");
                return;
            }

            _currentLabel = value.ToString();
        }

        public object GetValue()
        {
            return _label;
        }
        #endregion
        public override object GetValue(NodePort port)
        {
            return _output;
        }
        #region Save
        public object GetDataForSave()
        {
            throw new System.NotImplementedException();
        }

        public void SetDataFromSave(object data)
        {
            if (data is string inputData)
            {
                _output = inputData;
                _edited = true;
            }

            else
            {
                NovelGameDebug.LogError($"input fata for node {GUID} is invalid. Type: {data.GetType().Name}");
            }
        }

        public void ResetSaveBehaviour()
        {
            _edited = false;
        }
        #endregion
    }
}
