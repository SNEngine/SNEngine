using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.Graphs;
using System;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem.InstructionHandlers
{
    public class StartInstructionHandler : BaseInstructionHandler
    {
        public override bool CanHandle(string instruction)
        {
            return string.Equals(instruction.Trim(), "Start", StringComparison.OrdinalIgnoreCase);
        }

        public override InstructionResult Handle(string instruction, InstructionContext context)
        {
            if (context.Graph == null)
            {
                return InstructionResult.Error("Graph not initialized. 'name:' instruction must be processed before 'Start'.");
            }

            var dialogueGraph = (DialogueGraph)context.Graph;
            
            // Создаем StartNode
            var startNodeType = SNILTypeResolver.GetNodeType("StartNode");
            if (startNodeType == null)
            {
                return InstructionResult.Error("StartNode type not found.");
            }

            var startNode = dialogueGraph.AddNode(startNodeType) as BaseNode;
            if (startNode != null)
            {
                startNode.name = "Start";
                startNode.position = new Vector2(0, 0);

                // Применяем параметры, если есть (обычно нет для StartNode)
                var parameters = new System.Collections.Generic.Dictionary<string, string>();
                SNILParameterApplier.ApplyParametersToNode(startNode, parameters, "StartNode");

                AssetDatabase.AddObjectToAsset(startNode, dialogueGraph);
                context.Nodes.Add(startNode);

                // Сохраняем ссылку на StartNode в контексте для последующего использования
                if (context.Variables.ContainsKey("StartNode"))
                    context.Variables["StartNode"] = startNode;
                else
                    context.Variables.Add("StartNode", startNode);

                // Обновляем последнюю ноду
                context.LastNode = startNode;
            }

            return InstructionResult.Ok(startNode);
        }
    }
}