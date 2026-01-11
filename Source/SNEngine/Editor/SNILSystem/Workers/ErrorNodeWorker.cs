using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SiphoinUnityHelpers.Exceptions;
using SiphoinUnityHelpers.XNodeExtensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Editor.SNILSystem.Workers
{
    public class ErrorNodeWorker : SNILWorker
    {
        public override void ApplyParameters(BaseNode node, Dictionary<string, string> parameters)
        {
            // Проверяем, что это действительно ErrorNode или его наследник
            if (!(node is ErrorNode errorNode))
            {
                SNILDebug.LogError($"Node {node.GetType().Name} is not an ErrorNode");
                return;
            }

            // Устанавливаем параметры для ErrorNode
            foreach (var param in parameters)
            {
                switch (param.Key.ToLower())
                {
                    case "message":
                        SetFieldValue(errorNode, "_message", param.Value);
                        break;
                    case "exception":
                        // Создаем исключение или находим существующее
                        NodeException exception = new NodeException(param.Value);
                        SetFieldValue(errorNode, "_exception", exception);
                        break;
                    default:
                        SNILDebug.LogWarning($"Unknown parameter for ErrorNode: {param.Key}");
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
    }
}