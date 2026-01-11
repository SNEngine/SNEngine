using SiphoinUnityHelpers.XNodeExtensions;
using SiphoinUnityHelpers.XNodeExtensions.NodesControlExecutes;
using SNEngine.Editor.SNILSystem.FunctionSystem;
using SNEngine.Graphs;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem.InstructionHandlers
{
    public class CallInstructionHandler : BaseInstructionHandler
    {
        public override bool CanHandle(string instruction)
        {
            return Regex.IsMatch(instruction.Trim(), @"^call\s+.+", RegexOptions.IgnoreCase);
        }

        public override InstructionResult Handle(string instruction, InstructionContext context)
        {
            var (success, functionName) = ExtractValue(instruction, @"^call\s+(.+)");

            if (!success)
            {
                return InstructionResult.Error("Invalid call instruction format. Expected: 'call <function_name>'");
            }

            // Проверяем, существует ли функция
            if (!context.Functions.ContainsKey(functionName))
            {
                return InstructionResult.Error($"Function '{functionName}' not found.");
            }

            // Создаем GroupCallsNode для вызова функции
            if (context.Graph == null)
            {
                return InstructionResult.Error("Graph not initialized. Cannot create function call node.");
            }

            var dialogueGraph = (DialogueGraph)context.Graph;

            // Создаем GroupCallsNode (или другой тип ноды для вызова функции)
            var groupCallsNodeType = SNILTypeResolver.GetNodeType("GroupCallsNode");
            if (groupCallsNodeType == null)
            {
                return InstructionResult.Error("GroupCallsNode type not found.");
            }

            var callNode = dialogueGraph.AddNode(groupCallsNodeType) as BaseNode;
            if (callNode != null)
            {
                callNode.name = $"Call {functionName}";
                // Размещаем GroupCallsNode немного выше основного потока для визуального отделения
                callNode.position = new Vector2(context.Nodes.Count * 250, -150);

                // Устанавливаем имя функции как параметр
                var parameters = new Dictionary<string, string> { { "name", functionName } };
                SNILParameterApplier.ApplyParametersToNode(callNode, parameters, "GroupCallsNode");

                AssetDatabase.AddObjectToAsset(callNode, dialogueGraph);
                context.Nodes.Add(callNode);
                // Соединяем с предыдущей нодой
                NodeConnectionUtility.ConnectNodeToLast(dialogueGraph, callNode, context);

                // Создаем тело функции и подключаем его к GroupCallsNode
                if (context.Functions.ContainsKey(functionName))
                {
                    var function = context.Functions[functionName] as SNILFunction;
                    if (function != null)
                    {
                        // Создаем ноды тела функции
                        var functionBodyContext = new InstructionContext
                        {
                            Graph = context.Graph,
                            CurrentGraphName = context.CurrentGraphName,
                            Functions = context.Functions
                        };

                        var functionBodyNodes = FunctionBodyCreator.CreateFunctionBody(dialogueGraph, function, functionBodyContext);

                        // Подключаем первую ноду тела функции к порту _operations GroupCallsNode
                        if (functionBodyNodes.Count > 0)
                        {
                            var groupNode = callNode as GroupCallsNode;
                            if (groupNode != null)
                            {
                                var operationsPort = groupNode.GetOutputPort("_operations");
                                var firstNode = functionBodyNodes[0] as BaseNode;
                                var firstNodeInteraction = firstNode as BaseNodeInteraction;

                                if (operationsPort != null && firstNodeInteraction != null)
                                {
                                    var firstNodeEnterPort = firstNodeInteraction.GetEnterPort();
                                    if (firstNodeEnterPort != null)
                                    {
                                        operationsPort.Connect(firstNodeEnterPort);
                                    }
                                }

                                // Соединяем ноды тела функции последовательно
                                for (int i = 0; i < functionBodyNodes.Count - 1; i++)
                                {
                                    var currNode = functionBodyNodes[i] as BaseNodeInteraction;
                                    var nextNode = functionBodyNodes[i + 1] as BaseNodeInteraction;

                                    if (currNode != null && nextNode != null)
                                    {
                                        var outPort = currNode.GetExitPort();
                                        var inPort = nextNode.GetEnterPort();
                                        if (outPort != null && inPort != null)
                                        {
                                            outPort.Connect(inPort);
                                        }
                                    }
                                }
                            }

                            // Позиционируем ноды тела функции выше основного потока
                            PositionFunctionBodyNodes(callNode, functionBodyNodes);
                        }
                    }
                }

                return InstructionResult.Ok(callNode);
            }

            return InstructionResult.Error($"Failed to create function call node for function: {functionName}");
        }

        private static void PositionFunctionBodyNodes(BaseNode groupNode, List<object> bodyNodes)
        {
            if (bodyNodes.Count == 0) return;

            // Получаем позицию GroupCallsNode для определения стартовой точки
            float groupNodeX = groupNode.position.x;
            float groupNodeY = groupNode.position.y; // Это будет -150 или другое отрицательное значение

            // Размещаем ноды тела функции немного правее GroupCallsNode и выше основного потока
            for (int i = 0; i < bodyNodes.Count; i++)
            {
                var node = bodyNodes[i] as BaseNode;
                if (node != null)
                {
                    node.position = new Vector2(groupNodeX + (i + 1) * 250, groupNodeY - 50); // Ещё выше основного потока
                }
            }
        }
    }
}