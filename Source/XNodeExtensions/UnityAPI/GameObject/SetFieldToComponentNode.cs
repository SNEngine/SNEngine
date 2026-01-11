using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects
{
    public class SetFieldToComponentNode : BaseNodeInteraction
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private string _fieldName;

        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private Component _component;

        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private Object _value;

        [SerializeField] private FieldType _fieldType;

        public override void Execute()
        {
            Component component = GetInputValue<Component>(nameof(_component));

            string fieldName = _fieldName;

            object value = GetInputValue<object>(nameof(_value));

            string inputFieldName = GetDataFromPort<string>(nameof(_fieldName));

            if (inputFieldName != null)
            {
                fieldName = inputFieldName;
            }

            Type type = component.GetType();

            switch (_fieldType)
            {
                case FieldType.Field:
                    var field = type.GetField(fieldName);
                    field.SetValue(component, value);
                    break;
                case FieldType.Property:
                    var property = type.GetProperty(fieldName);
                    property.SetValue(component, value);
                    break;
            }
        }
    }
}
