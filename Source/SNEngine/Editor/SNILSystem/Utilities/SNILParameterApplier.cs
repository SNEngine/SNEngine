using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.CharacterSystem.Animations.Fade;
using SNEngine.DialogSystem;
using SNEngine.Editor.SNILSystem.Workers;
using SNEngine.SelectVariantsSystem;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Editor.SNILSystem
{
    public class SNILParameterApplier
    {
        private static readonly Dictionary<Type, Func<SNILWorker>> _nodeWorkerMap;
        private static readonly Dictionary<string, Func<SNILWorker>> _nodeNameWorkerMap;

        static SNILParameterApplier()
        {
            _nodeWorkerMap = new Dictionary<Type, Func<SNILWorker>>
            {
                { typeof(DialogNode), () => new DialogNodeWorker() },
                { typeof(DebugNode), () => new DebugNodeWorker() },
                { typeof(ErrorNode), () => new ErrorNodeWorker() },
                { typeof(ShowVariantsNode), () => new ShowVariantsNodeWorker() },
                { typeof(FadeCharacterNode), () => new FadeCharacterNodeWorker() },
                { typeof(FadeCharacterInOutNode), () => new FadeCharacterInOutNodeWorker() },
                // Ensure JumpToDialogueNode uses its specific worker so it registers pending jump references
                { typeof(JumpToDialogueNode), () => new JumpToDialogueNodeWorker() }
            };

            _nodeNameWorkerMap = new Dictionary<string, Func<SNILWorker>>(StringComparer.OrdinalIgnoreCase)
            {
                { "StartNode", () => new StartNodeWorker() },
                { "ExitNode", () => new ExitNodeWorker() }
            };
        }

        public static void ApplyParametersToNode(BaseNode node, Dictionary<string, string> parameters, string nodeName)
        {
            SNILWorker worker = GetWorkerForNode(node, nodeName);
            worker?.ApplyParameters(node, parameters);
        }

        public static void ApplyParametersToNodeGeneric(BaseNode node, Dictionary<string, string> parameters)
        {
            // Handle special case for ease parameter using the editor method
            if (parameters.ContainsKey("ease") || parameters.ContainsKey("_ease"))
            {
                string easeValue = parameters.ContainsKey("ease") ? parameters["ease"] : parameters["_ease"];

                // Try to call the ApplyEase_Editor method if it exists
                var applyEaseMethod = node.GetType().GetMethod("ApplyEase_Editor",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                if (applyEaseMethod != null)
                {
                    applyEaseMethod.Invoke(node, new object[] { easeValue });
                    // Remove the ease parameter so it's not processed as a field
                    parameters.Remove("ease");
                    parameters.Remove("_ease");
                }
            }

            // Общий метод для применения параметров через рефлексию
            System.Type type = node.GetType();
            var fields = GetAllFields(type);

            foreach (var kvp in parameters)
            {
                var field = fields.FirstOrDefault(f =>
                    f.Name.Equals(kvp.Key, System.StringComparison.OrdinalIgnoreCase) ||
                    f.Name.Equals("_" + kvp.Key, System.StringComparison.OrdinalIgnoreCase));

                if (field != null)
                {
                    object val = ConvertValue(kvp.Value, field.FieldType);
                    if (val != null || !field.FieldType.IsValueType)
                    {
                        field.SetValue(node, val);
                    }
                }
            }
        }

        private static SNILWorker GetWorkerForNode(BaseNode node, string nodeName)
        {
            // Сначала пытаемся получить информацию о воркере из шаблона
            var templateInfo = SNILTemplateManager.GetTemplateInfo(nodeName);
            if (!string.IsNullOrEmpty(templateInfo?.WorkerName))
            {
                // Используем рефлексию для создания экземпляра воркера по имени
                return CreateWorkerByName(templateInfo.WorkerName);
            }

            // Проверяем сопоставление по типу ноды
            if (_nodeWorkerMap.ContainsKey(node.GetType()))
            {
                return _nodeWorkerMap[node.GetType()]();
            }

            // Проверяем сопоставление по имени ноды
            if (_nodeNameWorkerMap.ContainsKey(nodeName))
            {
                return _nodeNameWorkerMap[nodeName]();
            }

            // Для других типов нод возвращаем общий воркер
            return new GenericNodeWorker();
        }

        private static SNILWorker CreateWorkerByName(string workerName)
        {
            // Пытаемся найти тип воркера в текущей сборке
            System.Type workerType = null;
            
            // Сначала ищем с полным именем класса
            workerType = System.Type.GetType($"SNEngine.Editor.SNILSystem.Workers.{workerName}");
            
            // Если не нашли, ищем в текущей сборке
            if (workerType == null)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    workerType = assembly.GetTypes()
                        .FirstOrDefault(t => t.Name == workerName && 
                                           typeof(Workers.SNILWorker).IsAssignableFrom(t));
                    if (workerType != null) break;
                }
            }

            if (workerType != null)
            {
                return (SNILWorker)Activator.CreateInstance(workerType);
            }

            // Если не удалось создать воркер по имени, возвращаем общий воркер
            return new GenericNodeWorker();
        }

        private static FieldInfo[] GetAllFields(System.Type type)
        {
            var fields = new List<FieldInfo>();
            while (type != null && type != typeof(object))
            {
                fields.AddRange(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
                type = type.BaseType;
            }
            return fields.ToArray();
        }

        private static object ConvertValue(string value, System.Type targetType)
        {
            if (targetType == typeof(string)) return value;
            if (targetType == typeof(int)) return int.TryParse(value, out int i) ? i : 0;
            if (targetType == typeof(float))
            {
                // Try parsing with different number styles to handle various formats like 0.0, 0,5, etc.
                if (float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float f))
                {
                    return f;
                }
                // Fallback to default parsing
                return float.TryParse(value, out float f2) ? f2 : 0f;
            }
            if (targetType == typeof(bool)) return bool.TryParse(value, out bool b) ? b : false;
            if (targetType.IsEnum)
            {
                try
                {
                    // Handle Ease enum specifically since it's from DOTween
                    if (targetType.Name == "Ease" && targetType.Namespace == "DG.Tweening")
                    {
                        // Try to parse the Ease enum value
                        return System.Enum.Parse(targetType, value, true);
                    }
                    return System.Enum.Parse(targetType, value, true);
                }
                catch
                {
                    // If parsing fails, try to get the first enum value as default
                    return System.Enum.GetValues(targetType).GetValue(0);
                }
            }

            // Обработка массива строк для _variants
            if (targetType == typeof(string[]))
            {
                // Используем ту же логику, что и в ShowVariantsNodeWorker
                return ParseOptionsString(value);
            }

            if (typeof(Object).IsAssignableFrom(targetType))
            {
                string filter = $"t:{targetType.Name} {value}";
                string[] guids = AssetDatabase.FindAssets(filter);
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    return AssetDatabase.LoadAssetAtPath(path, targetType);
                }
            }

            return null;
        }

        internal static string[] ParseOptionsString(string input)
        {
            // Check if the input is in array format (e.g., "[Option 1, Option 2, Option 3]")
            if (input.StartsWith("[") && input.EndsWith("]"))
            {
                // Extract content between brackets
                string arrayContent = input.Substring(1, input.Length - 2);

                // Split by comma, but handle nested commas properly by tracking brackets/quotes
                List<string> parts = new List<string>();
                int bracketLevel = 0;
                bool inDoubleQuote = false;
                bool inSingleQuote = false;
                int lastSplit = 0;

                for (int i = 0; i < arrayContent.Length; i++)
                {
                    char c = arrayContent[i];
                    bool isEscaped = (i > 0 && arrayContent[i-1] == '\\');

                    if (c == '[' && !isEscaped) bracketLevel++;
                    else if (c == ']' && !isEscaped) bracketLevel--;
                    else if (c == '"' && !isEscaped)
                    {
                        if (!inSingleQuote) // Only toggle double quotes if not inside single quotes
                        {
                            inDoubleQuote = !inDoubleQuote;
                        }
                    }
                    else if (c == '\'' && !isEscaped)
                    {
                        if (!inDoubleQuote) // Only toggle single quotes if not inside double quotes
                        {
                            inSingleQuote = !inSingleQuote;
                        }
                    }
                    else if (c == ',' && bracketLevel == 0 && !inDoubleQuote && !inSingleQuote)
                    {
                        parts.Add(arrayContent.Substring(lastSplit, i - lastSplit).Trim());
                        lastSplit = i + 1;
                    }
                }

                // Add the last part
                if (lastSplit < arrayContent.Length)
                {
                    parts.Add(arrayContent.Substring(lastSplit).Trim());
                }

                // Trim whitespace from each part and remove quotes if they exist
                for (int i = 0; i < parts.Count; i++)
                {
                    string part = parts[i].Trim();

                    // Remove surrounding quotes if they exist
                    if ((part.StartsWith("\"") && part.EndsWith("\"") && part.Length >= 2) ||
                        (part.StartsWith("'") && part.EndsWith("'") && part.Length >= 2))
                    {
                        part = part.Substring(1, part.Length - 2);
                    }

                    parts[i] = part;
                }

                return parts.ToArray();
            }
            // Check if the input uses dash separator format (e.g., " - Variant A - Variant B - Variant C")
            // If it contains the pattern " - " at the beginning, we'll treat it as dash-separated
            else if (input.Contains(" - "))
            {
                // Split by " - " (dash with spaces)
                string[] parts = input.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);

                // The first part might be empty if the string starts with " - ", so we remove it
                if (parts.Length > 0 && string.IsNullOrEmpty(parts[0].Trim()))
                {
                    // Remove the first empty element
                    List<string> partsList = new List<string>(parts);
                    partsList.RemoveAt(0);
                    parts = partsList.ToArray();
                }

                // Trim whitespace from each part
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i] = parts[i].Trim();
                }

                return parts;
            }
            else
            {
                // Original format: split by comma
                // Example: "Option 1: The safe choice, Option 2: The risky choice, Option 3: The mysterious choice"
                string[] parts = input.Split(',');

                // Trim whitespace from each part
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i] = parts[i].Trim();
                }

                return parts;
            }
        }
    }

    // Добавляем общий воркер для остальных нод
    public class GenericNodeWorker : Workers.SNILWorker
    {
        public override void ApplyParameters(BaseNode node, Dictionary<string, string> parameters)
        {
            // Применяем параметры общим способом через рефлексию
            SNILParameterApplier.ApplyParametersToNodeGeneric(node, parameters);
        }
    }
}