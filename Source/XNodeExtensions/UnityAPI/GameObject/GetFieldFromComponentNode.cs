using System;
using UnityEngine;
using XNode;
using Object = UnityEngine.Object;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects
{
    public class GetFieldFromComponentNode : BaseNode
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private string _fieldName;

        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private Component _component;

        [SerializeField] private FieldType _fieldType;

        [Output(ShowBackingValue.Never), SerializeField] private Object _value;

        private object _outputValue;

        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }

            Component component = GetInputValue<Component>(nameof(_component));

            string fieldName = _fieldName;

            string inputFieldName = GetDataFromPort<string>(nameof(_fieldName));

            if (inputFieldName != null)
            {
                fieldName = inputFieldName;
            }

            Type type = component.GetType();

            _outputValue = _fieldType == FieldType.Field ? type.GetField(fieldName).GetValue(component) : type.GetProperty(fieldName).GetValue(component);

            return _outputValue;
        }
    }
}
