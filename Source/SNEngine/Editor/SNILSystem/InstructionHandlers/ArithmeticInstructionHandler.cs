using SiphoinUnityHelpers.XNodeExtensions;
using SiphoinUnityHelpers.XNodeExtensions.Math.Arifmetic;
using SNEngine.Graphs;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem.InstructionHandlers
{
    public class ArithmeticInstructionHandler : BaseInstructionHandler
    {
        public override bool CanHandle(string instruction)
        {
            // Check for arithmetic operations like:
            // - variable++ (increment)
            // - variable-- (decrement)
            // - variable + value
            // - variable - value
            // - variable * value
            // - variable / value
            // - variable % value
            string pattern = @"^\w+\s*(\+\+|--|\+=|-=|\*=|/=|%=\s|\+|\-|\*|/|%)\s*.*$";
            return Regex.IsMatch(instruction.Trim(), pattern);
        }

        public override InstructionResult Handle(string instruction, InstructionContext context)
        {
            if (context.Graph == null)
            {
                return InstructionResult.Error("Graph not initialized.");
            }

            var dialogueGraph = (DialogueGraph)context.Graph;

            // Parse the arithmetic instruction
            var arithmeticInstruction = ParseArithmeticInstruction(instruction.Trim());
            if (arithmeticInstruction == null)
            {
                return InstructionResult.Error($"Invalid arithmetic instruction format: {instruction}");
            }

            // Find the variable in the context or graph
            VariableNode variableNode = FindVariableInContext(context, arithmeticInstruction.VariableName);
            if (variableNode == null)
            {
                variableNode = FindVariableByName(dialogueGraph, arithmeticInstruction.VariableName);
            }

            if (variableNode == null)
            {
                return InstructionResult.Error($"Variable '{arithmeticInstruction.VariableName}' not found. Must be declared before arithmetic operation.");
            }

            // Determine the appropriate arithmetic node type based on the variable type
            Type arithmeticNodeType = GetArithmeticNodeType(variableNode.GetType());
            if (arithmeticNodeType == null)
            {
                return InstructionResult.Error($"Arithmetic operations not supported for variable type: {variableNode.GetType().Name}");
            }

            // Create the arithmetic node
            var arithmeticNode = dialogueGraph.AddNode(arithmeticNodeType) as BaseNode;
            if (arithmeticNode == null)
            {
                return InstructionResult.Error("Failed to create arithmetic node.");
            }

            // Set the operation type based on the operator
            ArifmeticType operationType = GetArithmeticType(arithmeticInstruction.Operator);
            var parameters = new Dictionary<string, string>
            {
                { "_type", ((int)operationType).ToString() }
            };

            // Set the value for the second operand (B)
            if (arithmeticInstruction.Value != null)
            {
                parameters.Add("_b", arithmeticInstruction.Value);
            }

            // Apply parameters to the node
            SNILParameterApplier.ApplyParametersToNode(arithmeticNode, parameters, arithmeticNodeType.Name);

            // Position the arithmetic node - place it below the variable nodes and above the set nodes
            arithmeticNode.name = $"Arithmetic {arithmeticInstruction.VariableName}";

            // Use the same horizontal positioning approach as other execution nodes
            // Get the horizontal position based on the current execution flow position
            float baseX = 500; // Starting X position
            if (context.LastNode != null && context.LastNode is BaseNode lastBaseNode)
            {
                // Position this arithmetic operation after the last node in the execution flow
                baseX = lastBaseNode.position.x + 250; // Standard spacing
            }

            // Position arithmetic node at the base X position
            arithmeticNode.position = new Vector2(baseX, 200); // Arithmetic nodes go at y=200

            // Add the node to the asset
            AssetDatabase.AddObjectToAsset(arithmeticNode, dialogueGraph);

            // Add the node to the context
            context.Nodes.Add(arithmeticNode);

            // Connect the variable to the arithmetic node's A input
            ConnectVariableToArithmeticNode(dialogueGraph, variableNode, arithmeticNode, arithmeticInstruction.Operator);

            // Create a SetVariable node to store the result back to the variable
            var setNode = CreateSetVariableNode(dialogueGraph, variableNode, arithmeticNode, context);
            if (setNode == null)
            {
                return InstructionResult.Error("Failed to create SetVariable node for arithmetic result.");
            }

            // Position the set node at the same X as the arithmetic node for data flow visualization
            setNode.position = new Vector2(baseX, 350); // Set nodes go at y=350
            setNode.name = $"Set {arithmeticInstruction.VariableName}";

            // Update the context's position for the next node to continue the horizontal flow
            // The set node will be the one that continues the execution flow, so we need to make sure
            // the next instruction will be positioned after this set node
            // This is handled by NodeConnectionUtility.ConnectNodeToLast which updates context.LastNode

            // Connect the arithmetic node output to the SetVariable node (for data flow)
            ConnectArithmeticToSetNode(dialogueGraph, arithmeticNode, setNode);

            // For execution flow, the SetVariable node should be connected to the previous node in the execution flow
            // The arithmetic node is only for data flow, not execution flow
            NodeConnectionUtility.ConnectNodeToLast(dialogueGraph, setNode, context);

            return InstructionResult.Ok(setNode);
        }

        private ArithmeticInstruction ParseArithmeticInstruction(string instruction)
        {
            // Handle increment/decrement (e.g., number++)
            var incrementMatch = Regex.Match(instruction, @"^(\w+)\s*(\+\+)$", RegexOptions.IgnoreCase);
            if (incrementMatch.Success)
            {
                return new ArithmeticInstruction
                {
                    VariableName = incrementMatch.Groups[1].Value,
                    Operator = incrementMatch.Groups[2].Value,
                    Value = "1" // Default increment/decrement value
                };
            }

            // Handle decrement (e.g., number--)
            var decrementMatch = Regex.Match(instruction, @"^(\w+)\s*(--)$", RegexOptions.IgnoreCase);
            if (decrementMatch.Success)
            {
                return new ArithmeticInstruction
                {
                    VariableName = decrementMatch.Groups[1].Value,
                    Operator = decrementMatch.Groups[2].Value,
                    Value = "1" // Default increment/decrement value
                };
            }

            // Handle compound assignment (e.g., number += 5)
            var compoundMatch = Regex.Match(instruction, @"^(\w+)\s*([+\-*/%]=)\s*(.+)$", RegexOptions.IgnoreCase);
            if (compoundMatch.Success)
            {
                string op = compoundMatch.Groups[2].Value.Substring(0, 1); // Extract operator without =
                return new ArithmeticInstruction
                {
                    VariableName = compoundMatch.Groups[1].Value,
                    Operator = op,
                    Value = compoundMatch.Groups[3].Value.Trim()
                };
            }

            // Handle simple arithmetic (e.g., number + 5)
            var simpleMatch = Regex.Match(instruction, @"^(\w+)\s*([+\-*/%])\s*(.+)$", RegexOptions.IgnoreCase);
            if (simpleMatch.Success)
            {
                return new ArithmeticInstruction
                {
                    VariableName = simpleMatch.Groups[1].Value,
                    Operator = simpleMatch.Groups[2].Value,
                    Value = simpleMatch.Groups[3].Value.Trim()
                };
            }

            return null;
        }

        private ArifmeticType GetArithmeticType(string op)
        {
            switch (op)
            {
                case "+":
                case "++":
                    return ArifmeticType.Increment;
                case "-":
                case "--":
                    return ArifmeticType.Decrement;
                case "*":
                    return ArifmeticType.Multiply;
                case "/":
                    return ArifmeticType.Divide;
                case "%":
                    return ArifmeticType.Percent;
                default:
                    return ArifmeticType.Increment; // Default
            }
        }

        private Type GetArithmeticNodeType(Type variableNodeType)
        {
            // Map variable node types to arithmetic node types
            string variableTypeName = variableNodeType.Name;
            
            if (variableTypeName.Contains("Int"))
            {
                return typeof(ArifmeticIntNode);
            }
            else if (variableTypeName.Contains("Float"))
            {
                return typeof(ArifmeticFloatNode);
            }
            else if (variableTypeName.Contains("Long"))
            {
                return typeof(ArifmeticLongNode);
            }
            else if (variableTypeName.Contains("Uint"))
            {
                return typeof(ArifmeticuintNode);
            }
            else if (variableTypeName.Contains("Ulong"))
            {
                return typeof(ArifmeticulongNode);
            }
            
            return null;
        }

        private VariableNode FindVariableInContext(InstructionContext context, string variableName)
        {
            // Look for the variable in the context
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
            // Look for the variable in the graph
            foreach (var node in graph.nodes)
            {
                if (node is VariableNode variableNode && 
                    (variableNode.Name?.Equals(variableName, StringComparison.OrdinalIgnoreCase) == true ||
                     variableNode.name?.Equals(variableName, StringComparison.OrdinalIgnoreCase) == true))
                {
                    return variableNode;
                }
            }
            return null;
        }

        private void ConnectVariableToArithmeticNode(DialogueGraph graph, VariableNode variableNode, BaseNode arithmeticNode, string op)
        {
            // Connect the variable's output to the arithmetic node's A input
            var variableOutputPort = variableNode.GetOutputPort("_value");
            var arithmeticInputPort = arithmeticNode.GetInputPort("_a");

            if (variableOutputPort != null && arithmeticInputPort != null)
            {
                variableOutputPort.Connect(arithmeticInputPort);
                EditorUtility.SetDirty(graph);
            }

            // If it's an increment or decrement operation, connect the value port too
            if (op == "++" || op == "--")
            {
                var valueInputPort = arithmeticNode.GetInputPort("_b");
                if (valueInputPort != null)
                {
                    // The value is already set via parameters
                }
            }
        }

        private BaseNode CreateSetVariableNode(DialogueGraph graph, VariableNode variableNode, BaseNode arithmeticNode, InstructionContext context)
        {
            // Determine the SetVariable node type based on the variable type
            Type variableType = variableNode.GetType();
            string variableTypeName = variableType.Name;

            // Remove "Node" from the type name
            if (variableTypeName.EndsWith("Node"))
            {
                variableTypeName = variableTypeName.Substring(0, variableTypeName.Length - 4);
            }

            // Form the Set node type name
            string setNodeTypeName = "Set" + variableTypeName + "Node";
            Type setNodeType = SNILTypeResolver.GetNodeType(setNodeTypeName);

            if (setNodeType == null)
            {
                return null;
            }

            var setNode = graph.AddNode(setNodeType) as BaseNode;
            if (setNode == null)
            {
                return null;
            }

            // Set parameters for the SetVariable node
            var parameters = new Dictionary<string, string>
            {
                { "TargetGuid", variableNode.GUID }
            };

            SNILParameterApplier.ApplyParametersToNode(setNode, parameters, setNodeTypeName);

            setNode.name = $"Set {variableNode.Name ?? variableNode.name}";
            int setNodeCount = CountSetVariableNodes(context);
            setNode.position = new Vector2(500 + ((setNodeCount + CountArithmeticNodes(context)) * 300), 350);

            AssetDatabase.AddObjectToAsset(setNode, graph);

            return setNode;
        }

        private void ConnectArithmeticToSetNode(DialogueGraph graph, BaseNode arithmeticNode, BaseNode setNode)
        {
            // Connect the arithmetic node's output to the set node's value input
            var arithmeticOutputPort = arithmeticNode.GetOutputPort("_output");
            var setNodeValuePort = setNode.GetInputPort("_value");

            if (arithmeticOutputPort != null && setNodeValuePort != null)
            {
                arithmeticOutputPort.Connect(setNodeValuePort);
                EditorUtility.SetDirty(graph);
                AssetDatabase.SaveAssets();
            }
        }

        private int CountArithmeticNodes(InstructionContext context)
        {
            int count = 0;
            foreach (var node in context.Nodes)
            {
                if (node != null && IsArithmeticNode(node))
                {
                    count++;
                }
            }
            return count;
        }

        private bool IsArithmeticNode(object node)
        {
            if (node is BaseNode baseNode)
            {
                return baseNode.GetType().Name.Contains("Arifmetic") ||
                       baseNode.GetType().Name.Contains("Arithmetic");
            }
            return false;
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
                       baseNode.GetType().Name.EndsWith("Node");
            }
            return false;
        }
    }

    internal class ArithmeticInstruction
    {
        public string VariableName { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
}