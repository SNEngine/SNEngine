using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.Graphs;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem.InstructionHandlers
{
    public class SetVariableInstructionHandler : BaseInstructionHandler
    {
        public override bool CanHandle(string instruction)
        {
            // Проверяем, соответствует ли инструкция формату "set [имя] = [значение]"
            string pattern = @"^set\s+\w+\s*=.*$";
            return Regex.IsMatch(instruction.Trim(), pattern, RegexOptions.IgnoreCase);
        }

        public override InstructionResult Handle(string instruction, InstructionContext context)
        {
            if (context.Graph == null)
            {
                return InstructionResult.Error("Graph not initialized.");
            }

            var dialogueGraph = (DialogueGraph)context.Graph;

            // Извлекаем имя переменной и значение из инструкции
            var (variableName, valueExpression) = ParseSetInstruction(instruction.Trim());
            if (string.IsNullOrEmpty(variableName))
            {
                return InstructionResult.Error($"Invalid set instruction format: {instruction}");
            }

            // Определяем тип переменной по значению
            var (nodeType, convertedValue) = DetermineNodeTypeAndValue(valueExpression);
            if (nodeType == null)
            {
                return InstructionResult.Error($"Could not determine variable type for value: {valueExpression}");
            }

            // Пытаемся найти существующую переменную по GUID (через сохраненные переменные в контексте)
            VariableNode existingVariable = FindVariableInContext(context, variableName);

            // Если переменная не найдена в контексте, ищем в графе
            if (existingVariable == null)
            {
                existingVariable = FindVariableByName(dialogueGraph, variableName);
            }

            // Если переменная все еще не найдена, создаем новую
            if (existingVariable == null)
            {
                existingVariable = CreateVariableNode(dialogueGraph, variableName, nodeType, convertedValue);
                if (existingVariable == null)
                {
                    return InstructionResult.Error($"Failed to create variable node of type: {nodeType.Name}");
                }

                // Сохраняем ссылку на переменную в контексте
                if (context.Variables.ContainsKey(variableName))
                    context.Variables[variableName] = existingVariable;
                else
                    context.Variables.Add(variableName, existingVariable);
            }
            else
            {
                // Если переменная существует, обновляем её значение
                UpdateVariableValue(existingVariable, convertedValue);
            }

            // Создаем узел SetVariableNode соответствующего типа
            var setNodeType = GetSetNodeType(nodeType);
            if (setNodeType == null)
            {
                return InstructionResult.Error($"Could not find SetVariable node type for: {nodeType.Name}");
            }

            var setNode = dialogueGraph.AddNode(setNodeType) as BaseNode;
            if (setNode == null)
            {
                return InstructionResult.Error("Failed to create SetVariable node.");
            }

            setNode.name = $"Set {variableName}";
            // Позиционируем узел вдали от основного потока (горизонтальная цепочка на одном уровне Y)
            int setNodeCount = CountSetVariableNodes(context);
            setNode.position = new Vector2(500 + (setNodeCount * 272), 88); // Позиционируем как в примере - горизонтальная цепочка

            // Применяем параметры
            var parameters = new Dictionary<string, string>
            {
                { "TargetGuid", existingVariable.GUID }
            };

            // Если есть значение, устанавливаем его
            if (convertedValue != null)
            {
                parameters.Add("_value", convertedValue.ToString());
            }

            SNILParameterApplier.ApplyParametersToNode(setNode, parameters, setNodeType.Name);

            // Сохраняем узел в ассет
            AssetDatabase.AddObjectToAsset(setNode, dialogueGraph);

            // Добавляем узел в контекст
            context.Nodes.Add(setNode);

            // Соединяем с предыдущим узлом
            NodeConnectionUtility.ConnectNodeToLast(dialogueGraph, setNode, context);

            // Устанавливаем соединение между переменной и SetVariableNode
            ConnectVariableToSetNode(dialogueGraph, existingVariable, setNode);

            return InstructionResult.Ok(setNode);
        }

        private (string variableName, string valueExpression) ParseSetInstruction(string instruction)
        {
            // Ищем формат "set variableName = value"
            var match = Regex.Match(instruction, @"^set\s+(\w+)\s*=\s*(.*)$", RegexOptions.IgnoreCase);
            if (match.Success && match.Groups.Count > 2)
            {
                return (match.Groups[1].Value.Trim(), match.Groups[2].Value.Trim());
            }

            return (null, null);
        }

        private (Type nodeType, object value) DetermineNodeTypeAndValue(string valueExpression)
        {
            // Проверяем, является ли значение булевым
            if (bool.TryParse(valueExpression, out bool boolValue))
            {
                return (typeof(SiphoinUnityHelpers.XNodeExtensions.Variables.BoolNode), boolValue);
            }

            // Проверяем, является ли значение целым числом
            if (int.TryParse(valueExpression, out int intValue))
            {
                return (typeof(SiphoinUnityHelpers.XNodeExtensions.Variables.IntNode), intValue);
            }

            // Проверяем, является ли значение числом с плавающей точкой
            if (float.TryParse(valueExpression, out float floatValue))
            {
                return (typeof(SiphoinUnityHelpers.XNodeExtensions.Variables.FloatNode), floatValue);
            }

            // Проверяем, является ли значение строкой (если в кавычках)
            if ((valueExpression.StartsWith("\"") && valueExpression.EndsWith("\"")) ||
                (valueExpression.StartsWith("'") && valueExpression.EndsWith("'")))
            {
                string stringValue = valueExpression.Substring(1, valueExpression.Length - 2); // Убираем кавычки
                return (typeof(SiphoinUnityHelpers.XNodeExtensions.Variables.StringNode), stringValue);
            }

            // По умолчанию считаем строкой
            return (typeof(SiphoinUnityHelpers.XNodeExtensions.Variables.StringNode), valueExpression);
        }

        private VariableNode FindVariableInContext(InstructionContext context, string variableName)
        {
            // Ищем переменную в контексте по имени
            if (context.Variables.ContainsKey(variableName))
            {
                if (context.Variables[variableName] is VariableNode variableNode)
                {
                    return variableNode;
                }
            }

            return null;
        }

        private VariableNode FindVariableByName(DialogueGraph graph, string variableName)
        {
            // Ищем переменную по имени в графе
            foreach (var node in graph.nodes)
            {
                if (node is VariableNode variableNode)
                {
                    // Проверяем имя переменной - может быть в формате "Name (Type)" из-за цветного тега
                    if (IsMatchingVariableName(variableNode, variableName))
                    {
                        return variableNode;
                    }
                }
            }

            return null;
        }

        private bool IsMatchingVariableName(VariableNode variableNode, string targetName)
        {
            // Проверяем, совпадает ли имя переменной с целевым
            // Учитываем, что имя может быть в формате "Name (Type)" из-за цветного тега
            if (variableNode.Name != null && variableNode.Name.Equals(targetName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Проверяем обычное имя узла
            if (variableNode.name != null)
            {
                // Убираем тип из имени (в формате "Name (Type)")
                string nodeNameWithoutType = variableNode.name;
                int parenIndex = nodeNameWithoutType.IndexOf(" (");
                if (parenIndex > 0)
                {
                    nodeNameWithoutType = nodeNameWithoutType.Substring(0, parenIndex);
                }

                if (nodeNameWithoutType.Equals(targetName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private VariableNode CreateVariableNode(DialogueGraph graph, string variableName, Type variableType, object initialValue)
        {
            // Создаем узел переменной нужного типа
            var variableNode = graph.AddNode(variableType) as VariableNode;
            if (variableNode != null)
            {
                variableNode.Name = variableName; // Устанавливаем имя через свойство Name

                // Устанавливаем начальное значение
                if (initialValue != null)
                {
                    variableNode.SetValue(initialValue);
                }

                // Позиционируем переменную в левой части графа, как в примере
                // Если переменных уже много, смещаем по оси Y, чтобы не накладывались
                int existingVariableCount = CountExistingVariables(graph);
                variableNode.position = new Vector2(488, 424 + (existingVariableCount * 128)); // Позиционируем как в примере - левее и с вертикальным смещением

                AssetDatabase.AddObjectToAsset(variableNode, graph);
            }

            return variableNode;
        }

        private void UpdateVariableValue(VariableNode variableNode, object newValue)
        {
            // Обновляем значение существующей переменной
            if (newValue != null)
            {
                variableNode.SetValue(newValue);
            }
        }

        private Type GetSetNodeType(Type variableType)
        {
            // Определяем тип Set-нода по типу переменной
            string variableTypeName = variableType.Name;

            // Убираем "Node" из имени типа переменной
            if (variableTypeName.EndsWith("Node"))
            {
                variableTypeName = variableTypeName.Substring(0, variableTypeName.Length - 4);
            }

            // Формируем имя Set-нода
            string setNodeTypeName = "Set" + variableTypeName + "Node";

            return SNILTypeResolver.GetNodeType(setNodeTypeName);
        }

        private int CountExistingVariables(DialogueGraph graph)
        {
            int count = 0;
            foreach (var node in graph.nodes)
            {
                if (node is VariableNode)
                {
                    count++;
                }
            }
            return count;
        }

        private void ConnectVariableToSetNode(DialogueGraph graph, VariableNode variableNode, BaseNode setNode)
        {
            // Находим порт _Variable в SetVariableNode
            var variableInputPort = setNode.GetInputPort("_Variable");

            if (variableInputPort != null)
            {
                // Находим выходной порт переменной
                var variableOutputPort = variableNode.GetOutputPort("_value");

                if (variableOutputPort != null)
                {
                    // Соединяем выход переменной с входом SetVariableNode
                    variableOutputPort.Connect(variableInputPort);

                    // Сохраняем изменения
                    UnityEditor.EditorUtility.SetDirty(graph);
                    UnityEditor.AssetDatabase.SaveAssets();
                }
            }
        }

        private int CountSetVariableNodes(InstructionContext context)
        {
            int count = 0;
            foreach (var node in context.Nodes)
            {
                if (node != null && IsSetVariableNode(node))
                {
                    count++;
                }
            }
            return count;
        }

        private bool IsSetVariableNode(object node)
        {
            if (node is BaseNode baseNode)
            {
                return baseNode.GetType().Name.StartsWith("Set") &&
                       baseNode.GetType().Name.EndsWith("Node") &&
                       (baseNode.GetType().Namespace.Contains("Variables.Set") ||
                        baseNode.GetType().BaseType?.Namespace?.Contains("Variables.Set") == true);
            }
            return false;
        }
    }
}