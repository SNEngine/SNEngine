using System;
using SiphoinUnityHelpers.XNodeExtensions;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Debugging;
using SNEngine.Localization;
using UnityEngine;

namespace SNEngine
{
    public abstract class PrinterTextNode : AsyncNode, IPrinterNode, ILocalizationNode
    {
        [SerializeField, TextArea(10, 100)] private string _text = "Some Text";

        private string _currentText;

        public event Action<IPrinterNode> OnMessage;

        public override void Execute()
        {
            base.Execute();

            if (string.IsNullOrEmpty(_currentText))
                _currentText = _text;

            OnMessage?.Invoke(this);
        }

        public string GetText()
        {
            if (string.IsNullOrEmpty(_currentText))
                _currentText = _text;

            return TextParser.ParseWithProperties(
                _currentText,
                graph as BaseGraph
            );
        }

        public void MarkIsEnd()
        {
            StopTask();
        }

        #region Localization

        public object GetOriginalValue()
        {
            return _text;
        }

        public object GetValue()
        {
            return _text;
        }

        public void SetValue(object value)
        {
            if (value is not string)
            {
                NovelGameDebug.LogError(
                    $"Error SetValue for node {GetType().Name} GUID {GUID} type not a String"
                );
                return;
            }

            _currentText = value.ToString();
        }

        #endregion
    }
}