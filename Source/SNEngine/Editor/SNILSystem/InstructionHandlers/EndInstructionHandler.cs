using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem.InstructionHandlers
{
    public class EndInstructionHandler : BaseInstructionHandler
    {
        public override bool CanHandle(string instruction)
        {
            return string.Equals(instruction.Trim(), "End", StringComparison.OrdinalIgnoreCase);
        }

        public override InstructionResult Handle(string instruction, InstructionContext context)
        {
            if (context.Graph == null)
            {
                return InstructionResult.Error("Graph not initialized. 'name:' instruction must be processed before 'End'.");
            }

            var dialogueGraph = (DialogueGraph)context.Graph;

            // Создаем ExitNode (End нода)
            var exitNodeType = SNILTypeResolver.GetNodeType("ExitNode");
            if (exitNodeType == null)
            {
                return InstructionResult.Error("ExitNode type not found.");
            }

            var exitNode = dialogueGraph.AddNode(exitNodeType) as BaseNode;
            if (exitNode != null)
            {
                exitNode.name = "End";
                
                // Позиционируем End node правее всех других нод в графе
                float maxXPosition = FindMaxXPosition(dialogueGraph) + 250; // Размещаем правее всех нод
                exitNode.position = new Vector2(maxXPosition, 0);

                // Применяем параметры, если есть
                var parameters = new Dictionary<string, string>();
                SNILParameterApplier.ApplyParametersToNode(exitNode, parameters, "ExitNode");

                AssetDatabase.AddObjectToAsset(exitNode, dialogueGraph);
                context.Nodes.Add(exitNode);

                // Сохраняем ссылку на EndNode в контексте
                if (context.Variables.ContainsKey("EndNode"))
                    context.Variables["EndNode"] = exitNode;
                else
                    context.Variables.Add("EndNode", exitNode);

                // Connect the exit node to the previous node in the execution flow
                NodeConnectionUtility.ConnectNodeToLast(dialogueGraph, exitNode, context);
            }

            return InstructionResult.Ok(exitNode);
        }

        private float FindMaxXPosition(DialogueGraph graph)
        {
            float maxX = 0;
            foreach (var node in graph.nodes)
            {
                if (node is BaseNode baseNode)
                {
                    maxX = Mathf.Max(maxX, baseNode.position.x);
                }
            }
            return maxX;
        }
    }
}