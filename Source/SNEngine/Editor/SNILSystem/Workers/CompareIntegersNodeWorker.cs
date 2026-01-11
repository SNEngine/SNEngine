using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SiphoinUnityHelpers.XNodeExtensions;
using SiphoinUnityHelpers.XNodeExtensions.Math.Compare;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Editor.SNILSystem.Workers
{
    public class CompareIntegersNodeWorker : SNILWorker
    {
        public override void ApplyParameters(BaseNode node, Dictionary<string, string> parameters)
        {
            // Проверяем, что это действительно CompareIntegersNode или его наследник
            if (!(node is CompareIntegersNode compareNode))
            {
                SNILDebug.LogError($"Node {node.GetType().Name} is not a CompareIntegersNode");
                return;
            }

            // Устанавливаем параметры для CompareIntegersNode
            foreach (var param in parameters)
            {
                switch (param.Key.ToLower())
                {
                    case "a":
                        SetFieldValue(compareNode, "_a", ParseValue(param.Value, typeof(int)));
                        break;
                    case "b":
                        SetFieldValue(compareNode, "_b", ParseValue(param.Value, typeof(int)));
                        break;
                    case "type":
                        if (System.Enum.TryParse<CompareType>(param.Value, true, out CompareType compareType))
                        {
                            SetFieldValue(compareNode, "_type", compareType);
                        }
                        else
                        {
                            SNILDebug.LogWarning($"Invalid CompareType value: {param.Value}. Using default Equals.");
                            SetFieldValue(compareNode, "_type", CompareType.Equals);
                        }
                        break;
                    default:
                        SNILDebug.LogWarning($"Unknown parameter for CompareIntegersNode: {param.Key}");
                        break;
                }
            }
        }

        private void SetFieldValue(Object obj, string fieldName, object value)
        {
            FieldInfo field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
        }

        private object ParseValue(string value, System.Type targetType)
        {
            if (targetType == typeof(int))
            {
                if (int.TryParse(value, out int result))
                    return result;
                else
                    return 0; // default value
            }
            else if (targetType == typeof(string))
            {
                return value;
            }
            
            return null;
        }
    }
}