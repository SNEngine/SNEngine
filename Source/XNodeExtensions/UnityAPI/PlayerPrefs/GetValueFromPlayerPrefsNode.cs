using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using System;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.PlayerPrefsSystem
{
    [NodeTint("#3d6b47")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/PlayerPrefs/Get Value")]
    public class GetValueFromPlayerPrefsNode : BaseNode
    {
        [Input, SerializeField, ReadOnly(ReadOnlyMode.OnEditor)] private string _key;

        [SerializeField, ReadOnly(ReadOnlyMode.OnEditor)] private FieldType _type;

        [Space(10)]
        [Output(ShowBackingValue.Never), SerializeField] private UnityEngine.Object _output;

        public override object GetValue(NodePort port)
        {
            var currentKey = _key;

            var keyInput = GetDataFromPort<string>(nameof(_key));

            if (keyInput != null)
            {
                currentKey = keyInput;
            }
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }

            if (!PlayerPrefs.HasKey(currentKey))
            {
                Debug.LogError($"key {currentKey} not found on PlayerPrefs");

                return base.GetValue(port);
            }
            switch (_type)
            {
                case FieldType.Int:
                    return PlayerPrefs.GetInt(currentKey);
                case FieldType.UInt:
                    return PlayerPrefs.GetInt(currentKey);
                case FieldType.String:
                    return PlayerPrefs.GetString(currentKey);
                case FieldType.Float:
                    return PlayerPrefs.GetFloat(currentKey);
                case FieldType.Double:
                    return PlayerPrefs.GetFloat(currentKey);
                case FieldType.Long:
                    return Convert.ToInt64(PlayerPrefs.GetString(currentKey));
                case FieldType.ULong:
                    return Convert.ToInt64(PlayerPrefs.GetString(currentKey));
                case FieldType.Bool:
                    return Convert.ToBoolean(PlayerPrefs.GetInt(currentKey));
                default:
                    break;
            }

            return base.GetValue(port);
        }


    }
}
