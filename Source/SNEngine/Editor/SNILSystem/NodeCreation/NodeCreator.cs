using System;
using System.Collections.Generic;
using System.Linq;
using SNEngine.Graphs;
using SNEngine.Editor.SNILSystem.FunctionSystem;
using UnityEditor;
using UnityEngine;
using XNode;
using SiphoinUnityHelpers.XNodeExtensions;
using SiphoinUnityHelpers.XNodeExtensions.NodesControlExecutes;

namespace SNEngine.Editor.SNILSystem.NodeCreation
{
    public class NodeCreator
    {
        public static List<BaseNode> CreateMainNodes(DialogueGraph graph, List<SNILInstruction> mainInstructions)
        {
            List<BaseNode> mainNodes = new List<BaseNode>();

            for (int i = 0; i < mainInstructions.Count; i++)
            {
                var inst = mainInstructions[i];
                if (inst.NodeType == null)
                {
                    SNILDebug.LogWarning($"Skipping instruction: {inst.NodeTypeName} - NodeType not resolved. Parameters: {string.Join(", ", inst.Parameters?.Select(kv => kv.Key + "=" + kv.Value) ?? new string[0])}");
                    continue;
                }

                SNILDebug.Log($"Creating node: {inst.NodeTypeName} ({inst.NodeType?.Name ?? "<null>"})");

                BaseNode node = graph.AddNode(inst.NodeType) as BaseNode;

                string displayName = NodeFormatter.FormatNodeDisplayName(inst.NodeTypeName);
                node.name = displayName;

                // Размещаем StartNode в начале
                if (inst.NodeTypeName.Equals("StartNode", System.StringComparison.OrdinalIgnoreCase))
                {
                    node.position = new Vector2(0, 0);
                }
                else
                {
                    node.position = new Vector2((i + 1) * 250, 0); // Сдвигаем остальные ноды
                }

                SNILParameterApplier.ApplyParametersToNode(node, inst.Parameters, inst.NodeTypeName);

                AssetDatabase.AddObjectToAsset(node, graph);
                mainNodes.Add(node);
            }

            return mainNodes;
        }

        public static (Dictionary<string, GroupCallsNode> functionMap, Dictionary<string, List<BaseNode>> functionBodies) 
            CreateFunctionNodes(DialogueGraph graph, List<SNILInstruction> functionInstructions)
        {
            Dictionary<string, GroupCallsNode> functionMap = new Dictionary<string, GroupCallsNode>();
            Dictionary<string, List<BaseNode>> functionBodies = new Dictionary<string, List<BaseNode>>();

            if (functionInstructions == null) return (functionMap, functionBodies);

            // Сначала создаем GroupCallsNode для каждой функции и сохраняем их в маппинг
            var groupCallInstructions = functionInstructions.Where(inst => inst.NodeTypeName == "GroupCallsNode").ToList();

            foreach (var inst in groupCallInstructions)
            {
                if (inst.NodeType == null) continue;

                GroupCallsNode node = graph.AddNode(inst.NodeType) as GroupCallsNode;

                string displayName = NodeFormatter.FormatNodeDisplayName(inst.NodeTypeName);
                node.name = displayName;
                // Размещаем GroupCallsNode на том же уровне, что и основной поток, но чуть выше для визуального отделения
                node.position = new Vector2(0, -150); // Временно ставим в нулевую позицию, переместим позже

                SNILParameterApplier.ApplyParametersToNode(node, inst.Parameters, inst.NodeTypeName);

                AssetDatabase.AddObjectToAsset(node, graph);

                // Сохраняем соответствие имя функции -> GroupCallsNode
                if (inst.Parameters.ContainsKey("name"))
                {
                    string functionName = inst.Parameters["name"];
                    functionMap[functionName] = node;
                    functionBodies[functionName] = new List<BaseNode>(); // Инициализируем пустое тело функции
                }
            }

            // Затем создаем ноды тела функций и распределяем их по соответствующим функциям
            var functionBodyInstructions = functionInstructions.Where(inst => inst.NodeTypeName != "GroupCallsNode").ToList();

            string currentFunctionName = null;
            foreach (var inst in functionInstructions)
            {
                if (inst.NodeTypeName == "GroupCallsNode" && inst.Parameters.ContainsKey("name"))
                {
                    currentFunctionName = inst.Parameters["name"];
                }
                else if (inst.NodeTypeName != "GroupCallsNode" && currentFunctionName != null)
                {
                    BaseNode node = graph.AddNode(inst.NodeType) as BaseNode;

                    string displayName = NodeFormatter.FormatNodeDisplayName(inst.NodeTypeName);
                    node.name = displayName;
                    // Размещаем тело функций выше основного потока, чтобы не мешало
                    node.position = new Vector2(0, -100); // Временная позиция, будет обновлена позже

                    SNILParameterApplier.ApplyParametersToNode(node, inst.Parameters, inst.NodeTypeName);

                    AssetDatabase.AddObjectToAsset(node, graph);
                    functionBodies[currentFunctionName].Add(node); // Добавляем ноду к телу текущей функции
                }
            }

            return (functionMap, functionBodies);
        }
    }
}