using System.Collections.Generic;
using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.Graphs;
using SNEngine.Editor.SNILSystem.InstructionHandlers;
using SNEngine.Editor.SNILSystem.FunctionSystem;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem.InstructionHandlers
{
    public static class FunctionBodyCreator
    {
        public static List<object> CreateFunctionBody(DialogueGraph graph, SNILFunction function, InstructionContext sharedContext)
        {
            var functionBodyNodes = new List<object>();

            // Создаем отдельный контекст для обработки тела функции
            var functionContext = new InstructionContext
            {
                Graph = sharedContext.Graph,
                CurrentGraphName = sharedContext.CurrentGraphName,
                Functions = sharedContext.Functions, // Доступ к функциям для вложенных вызовов
                Variables = sharedContext.Variables // Доступ к переменным
            };

            // Обрабатываем инструкции тела функции
            foreach (string instruction in function.Body)
            {
                string trimmedInstruction = instruction.Trim();

                if (string.IsNullOrEmpty(trimmedInstruction) || IsCommentLine(trimmedInstruction))
                    continue;

                // Используем обработчики инструкций для создания нод тела функции
                var result = InstructionHandlerManager.Instance.ProcessInstruction(trimmedInstruction, functionContext);

                if (result.Success && result.Data != null)
                {
                    var node = result.Data as BaseNode;
                    if (node != null)
                    {
                        functionBodyNodes.Add(node);
                    }
                }
            }

            return functionBodyNodes;
        }

        private static bool IsCommentLine(string line)
        {
            string trimmed = line.Trim();
            return trimmed.StartsWith("//") || trimmed.StartsWith("#");
        }
    }
}