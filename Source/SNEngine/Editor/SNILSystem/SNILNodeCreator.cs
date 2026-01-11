using System;
using System.Collections.Generic;
using SNEngine.Graphs;
using SNEngine.Editor.SNILSystem.NodeCreation;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem
{
    public class SNILNodeCreator
    {
        public static void CreateNodesFromInstructions(DialogueGraph graph, List<SNILInstruction> mainInstructions, List<SNILInstruction> functionInstructions = null, List<int> functionCallPositions = null, List<string> functionCallNames = null)
        {
            // Create main nodes
            var mainNodes = NodeCreator.CreateMainNodes(graph, mainInstructions);

            // Create function nodes
            var (functionMap, functionBodies) = NodeCreator.CreateFunctionNodes(graph, functionInstructions);

            // Connect function bodies to group nodes
            foreach (var functionBody in functionBodies)
            {
                if (functionBody.Value.Count > 0 && functionMap.ContainsKey(functionBody.Key))
                {
                    var groupNode = functionMap[functionBody.Key];
                    NodeConnector.ConnectFunctionBodyToGroup(groupNode, functionBody.Value);
                }
            }

            // Build complete node sequence
            var completeNodeSequence = NodePositioner.BuildCompleteNodeSequence(mainNodes, functionCallPositions, functionCallNames, functionMap);

            // Position all nodes
            NodePositioner.PositionNodesHorizontally(completeNodeSequence);

            // Connect all nodes sequentially
            NodeConnector.ConnectNodesSequentially(completeNodeSequence);
        }
    }
}