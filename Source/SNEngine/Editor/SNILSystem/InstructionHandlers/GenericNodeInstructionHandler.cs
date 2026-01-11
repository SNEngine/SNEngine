using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.Graphs;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem.InstructionHandlers
{
    public class GenericNodeInstructionHandler : BaseInstructionHandler
    {
        public override bool CanHandle(string instruction)
        {
            string trimmed = instruction.Trim();

            // Не обрабатываем lowercase "end" - это для завершения функций
            if (string.Equals(trimmed, "end", StringComparison.Ordinal))
            {
                return false;
            }

            // Не обрабатываем uppercase "End" - это обрабатывается EndInstructionHandler
            if (string.Equals(trimmed, "End", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Проверяем, может ли инструкция соответствовать шаблону ноды
            var templates = SNILTemplateManager.GetNodeTemplates();
            foreach (var template in templates)
            {
                var parameters = SNILTemplateMatcher.MatchLineWithTemplate(instruction, template.Value.Template);
                if (parameters != null)
                {
                    return true;
                }
            }

            // Проверяем команду Jump To
            if (trimmed.StartsWith("Jump To ", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public override InstructionResult Handle(string instruction, InstructionContext context)
        {
            if (context.Graph == null)
            {
                return InstructionResult.Error("Graph not initialized. 'name:' instruction must be processed before node instructions.");
            }

            var dialogueGraph = (DialogueGraph)context.Graph;
            var templates = SNILTemplateManager.GetNodeTemplates();

            // Попробуем сопоставить инструкцию с шаблоном
            foreach (var template in templates)
            {
                var parameters = SNILTemplateMatcher.MatchLineWithTemplate(instruction, template.Value.Template);
                if (parameters != null)
                {
                    // Найден подходящий шаблон, создаем ноду
                    return CreateNodeFromTemplate(dialogueGraph, template.Key, parameters, context);
                }
            }

            // Если не найдено подходящего шаблона, проверим специальные команды
            string trimmed = instruction.Trim();
            if (trimmed.StartsWith("Jump To ", StringComparison.OrdinalIgnoreCase))
            {
                // Обработка Jump To команды
                string targetDialogue = trimmed.Substring("Jump To ".Length).Trim();

                // Создаем JumpNode
                var jumpNodeType = SNILTypeResolver.GetNodeType("JumpNode");
                if (jumpNodeType != null)
                {
                    var jumpNode = dialogueGraph.AddNode(jumpNodeType) as BaseNode;
                    if (jumpNode != null)
                    {
                        jumpNode.name = $"Jump To {targetDialogue}";
                        jumpNode.position = new Vector2(context.Nodes.Count * 250, 0);

                        var jumpParams = new Dictionary<string, string> { { "_dialogue", targetDialogue } };
                        // Use the actual runtime node type name so we pick correct template/worker (e.g., JumpToDialogueNode)
                        SNILParameterApplier.ApplyParametersToNode(jumpNode, jumpParams, jumpNode.GetType().Name);

                        AssetDatabase.AddObjectToAsset(jumpNode, dialogueGraph);
                        context.Nodes.Add(jumpNode);
                        // Соединяем с предыдущей нодой
                        NodeConnectionUtility.ConnectNodeToLast(dialogueGraph, jumpNode, context);

                        return InstructionResult.Ok(jumpNode);
                    }
                }
                else
                {
                    // Если специфичного типа нет, создаем общую ноду с параметрами
                    var genericNodeType = SNILTypeResolver.GetNodeType("GenericNode"); // Или другой подходящий тип
                    if (genericNodeType != null)
                    {
                        var jumpNode = dialogueGraph.AddNode(genericNodeType) as BaseNode;
                        if (jumpNode != null)
                        {
                            jumpNode.name = $"Jump To {targetDialogue}";
                            jumpNode.position = new Vector2(context.Nodes.Count * 250, 0);

                            var jumpParams = new Dictionary<string, string> { { "_dialogue", targetDialogue } };
                            // Use the actual runtime node type name so we pick correct template/worker
                            SNILParameterApplier.ApplyParametersToNode(jumpNode, jumpParams, jumpNode.GetType().Name);

                            AssetDatabase.AddObjectToAsset(jumpNode, dialogueGraph);
                            context.Nodes.Add(jumpNode);
                            // Соединяем с предыдущей нодой
                            NodeConnectionUtility.ConnectNodeToLast(dialogueGraph, jumpNode, context);

                            return InstructionResult.Ok(jumpNode);
                        }
                    }
                }

                return InstructionResult.Error("JumpNode type not found.");
            }

            return InstructionResult.Error($"No matching template found for instruction: {instruction}");
        }

        private InstructionResult CreateNodeFromTemplate(DialogueGraph graph, string nodeTypeName, Dictionary<string, string> parameters, InstructionContext context = null)
        {
            var nodeType = SNILTypeResolver.GetNodeType(nodeTypeName);
            if (nodeType == null)
            {
                return InstructionResult.Error($"{nodeTypeName} type not found.");
            }

            var node = graph.AddNode(nodeType) as BaseNode;
            if (node != null)
            {
                string displayName = FormatNodeDisplayName(nodeTypeName);
                node.name = displayName;
                node.position = new Vector2(graph.nodes.Count * 250, 0); // Простое позиционирование

                SNILParameterApplier.ApplyParametersToNode(node, parameters, nodeTypeName);

                AssetDatabase.AddObjectToAsset(node, graph);

                // Если контекст предоставлен, добавляем ноду и соединяем
                if (context != null)
                {
                    context.Nodes.Add(node);
                    // Соединяем с предыдущей нодой
                    NodeConnectionUtility.ConnectNodeToLast(graph, node, context);
                }

                return InstructionResult.Ok(node);
            }

            return InstructionResult.Error($"Failed to create node of type: {nodeTypeName}");
        }

        private string FormatNodeDisplayName(string nodeTypeName)
        {
            // Преобразуем "ShowCharacter" в "Show Character"
            // Добавляем пробел перед заглавной буквой, если перед ней есть строчная буква
            if (string.IsNullOrEmpty(nodeTypeName)) return nodeTypeName;

            var result = new System.Text.StringBuilder();
            result.Append(nodeTypeName[0]); // Первую букву добавляем как есть

            for (int i = 1; i < nodeTypeName.Length; i++)
            {
                char currentChar = nodeTypeName[i];
                char prevChar = nodeTypeName[i - 1];

                // Если текущий символ - заглавная буква, а предыдущий - строчная, вставляем пробел
                if (char.IsUpper(currentChar) && char.IsLower(prevChar))
                {
                    result.Append(' ');
                }
                result.Append(currentChar);
            }

            string displayName = result.ToString();

            // Убираем "Node" из конца, если оно есть
            if (displayName.EndsWith("Node"))
                displayName = displayName.Substring(0, displayName.Length - 4);

            return displayName;
        }
    }
}