using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using System;
using System.Numerics;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.PlayerPrefsSystem
{
    [NodeTint("#3d6b47")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/PlayerPrefs/Set Value")]
    public class SetValueToPlayerPrefsNode : BaseNodeInteraction
    {
        [Input, SerializeField, ReadOnly(ReadOnlyMode.OnEditor)] private string _key = Guid.NewGuid().ToString();

        [Input, SerializeField, ReadOnly(ReadOnlyMode.OnEditor)] private string _value = "0";

        [SerializeField, ReadOnly(ReadOnlyMode.OnEditor)] private FieldType _type;

        public object GetValue ()
        {
            switch (_type)
            {
                case FieldType.Int:
                    return Convert.ToInt32(_value);
                    case FieldType.Float:
                    return Convert.ToSingle(_value);
                    case FieldType.String:
                    return _value;
                    case FieldType.Double:
                    return Convert.ToDouble(_value);
                    case FieldType.Long: 
                    return Convert.ToInt64(_value);
                    case FieldType.UInt:
                    return Convert.ToUInt32(_value);
                    case FieldType.ULong: 
                    return Convert.ToUInt64(_value);
                    case FieldType.Bool:

                    string validValue = _value.Replace(".", ",");

                    return Convert.ToBoolean(validValue);

                    default:
                    return new InvalidCastException($"invalid field type {_type} on node {GUID}");

            }
        }

        public override void Execute()
        {
            var currentValue = _value;

            var currentKey = _key;

            var valueInput = GetDataFromPort<string>(nameof(_value));

            if (valueInput != null)
            {
                currentValue = valueInput;
            }

            var keyInput = GetDataFromPort<string>(nameof(_value));

            if (keyInput != null)
            {
                currentKey = keyInput;
            }

            if (string.IsNullOrEmpty(_key))
            {
                throw new InvalidOperationException($"key invalid on node {GUID}");
            }

            if (string.IsNullOrEmpty(_value))
            {
                throw new InvalidOperationException($"value invalid on node {GUID}");
            }

            if (_type == FieldType.Int || _type == FieldType.UInt)
            {
                PlayerPrefs.SetInt(currentKey, (int)GetValue());
            }

            else if (_type == FieldType.Long || _type == FieldType.ULong)
            {
                PlayerPrefs.SetInt(currentKey, Convert.ToInt32(_value));
            }
            else if (_type == FieldType.Float)
            {
                PlayerPrefs.SetFloat(currentKey, (float)GetValue());
            }

            else if (_type == FieldType.Double)
            {
                PlayerPrefs.SetFloat(currentKey, (float)GetValue());
            }

            else if (_type == FieldType.String)
            {
                PlayerPrefs.SetString(currentKey, currentValue);
            }

            else if (_type == FieldType.Bool)
            {
                bool value = (bool)GetValue();

                PlayerPrefs.SetInt(currentKey, Convert.ToInt32(value));
            }


            PlayerPrefs.Save();
        }

        private void OnValidate()
        {
            if (_type != FieldType.String)
            {
                _value = _value.Trim();
            }
        }
    }
}
