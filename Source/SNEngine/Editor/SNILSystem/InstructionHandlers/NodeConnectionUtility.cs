using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.Graphs;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem.InstructionHandlers
{
    public static class NodeConnectionUtility
    {
        public static void ConnectNodesSequentially(DialogueGraph graph, List<object> nodes)
        {
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                var currentNode = nodes[i] as BaseNode;
                var nextNode = nodes[i + 1] as BaseNode;

                // Only connect sequential nodes that are part of the main flow (not nodes controlled by If/Loop/Group)
                if (currentNode is BaseNodeInteraction currentInteraction && 
                    nextNode is BaseNodeInteraction nextInteraction)
                {
                    // Skip nodes that are controlled by another node (e.g., branch nodes) to avoid cross-branch wiring
                    if (currentInteraction.IsControlledAnotherNode || nextInteraction.IsControlledAnotherNode)
                    {
                        continue;
                    }

                    var outPort = currentInteraction.GetExitPort();
                    var inPort = nextInteraction.GetEnterPort();
                    
                    if (outPort != null && inPort != null)
                    {
                        outPort.Connect(inPort);
                    }
                }
            }
            
            EditorUtility.SetDirty(graph);
            AssetDatabase.SaveAssets();
        }

        public static void ConnectNodeToLast(DialogueGraph graph, object newNode, InstructionContext context)
        {
            if (context.LastNode != null && newNode != null)
            {
                var prevNode = context.LastNode as BaseNode;
                var currNode = newNode as BaseNode;

                if (prevNode is BaseNodeInteraction prevInteraction &&
                    currNode is BaseNodeInteraction currInteraction)
                {
                    var outPort = prevInteraction.GetExitPort();
                    var inPort = currInteraction.GetEnterPort();

                    if (outPort != null && inPort != null)
                    {
                        outPort.Connect(inPort);
                    }
                }
            }

            context.LastNode = newNode;
        }
    }
}