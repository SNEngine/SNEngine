using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SiphoinUnityHelpers.XNodeExtensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Editor.SNILSystem.Workers
{
    public class DebugNodeWorker : SNILWorker
    {
        public override void ApplyParameters(BaseNode node, Dictionary<string, string> parameters)
        {
            // Проверяем, что это действительно DebugNode или его наследник
            if (!(node is DebugNode debugNode))
            {
                SNILDebug.LogError($"Node {node.GetType().Name} is not a DebugNode");
                return;
            }

            // Устанавливаем параметры для DebugNode
            foreach (var param in parameters)
            {
                switch (param.Key.ToLower())
                {
                    case "message":
                        SetFieldValue(debugNode, "_message", param.Value);
                        break;
                    case "target":
                        // Для targetLog пытаемся найти объект по имени
                        Object targetObject = FindObjectByName(param.Value);
                        if (targetObject != null)
                        {
                            SetFieldValue(debugNode, "_targetLog", targetObject);
                        }
                        else
                        {
                            SNILDebug.LogWarning($"Object with name '{param.Value}' not found for DebugNode target parameter");
                        }
                        break;
                    case "logtype":
                        if (System.Enum.TryParse<SiphoinUnityHelpers.XNodeExtensions.LogType>(param.Value, true, out SiphoinUnityHelpers.XNodeExtensions.LogType logType))
                        {
                            SetFieldValue(debugNode, "_logType", logType);
                        }
                        else
                        {
                            SNILDebug.LogWarning($"Invalid LogType value: {param.Value}");
                        }
                        break;
                    default:
                        SNILDebug.LogWarning($"Unknown parameter for DebugNode: {param.Key}");
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

        private Object FindObjectByName(string name)
        {
            // Ищем объекты в сцене по имени
            Object[] objects = Resources.FindObjectsOfTypeAll<Object>();
            foreach (Object obj in objects)
            {
                if (obj.name == name)
                {
                    return obj;
                }
            }
            
            // Если не найдено в сцене, пробуем найти через AssetDatabase
            if (AssetDatabase.IsValidFolder($"Assets/{name}") || AssetDatabase.LoadAssetAtPath<Object>($"Assets/{name}"))
            {
                return AssetDatabase.LoadAssetAtPath<Object>($"Assets/{name}");
            }
            
            return null;
        }
    }
}