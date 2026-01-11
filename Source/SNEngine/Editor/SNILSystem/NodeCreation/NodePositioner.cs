using System;
using System.Collections.Generic;
using System.Linq;
using SiphoinUnityHelpers.XNodeExtensions;
using SiphoinUnityHelpers.XNodeExtensions.NodesControlExecutes;
using SNEngine.Graphs;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem.NodeCreation
{
    public class NodePositioner
    {
        public static void PositionFunctionBodyNodes(GroupCallsNode groupNode, List<BaseNode> bodyNodes)
        {
            if (bodyNodes.Count == 0) return;

            // Получаем позицию GroupCallsNode для определения стартовой точки
            float groupNodeX = groupNode.position.x;
            float groupNodeY = groupNode.position.y; // Это будет -150 или другое отрицательное значение

            // Размещаем ноды тела функции немного правее GroupCallsNode и выше основного потока
            for (int i = 0; i < bodyNodes.Count; i++)
            {
                bodyNodes[i].position = new Vector2(groupNodeX + (i + 1) * 250, groupNodeY - 50); // Ещё выше основного потока
            }
        }

        public static void PositionNodesHorizontally(List<BaseNode> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                // Устанавливаем позицию на основном уровне (y=0) с интервалом 250 пикселей
                node.position = new Vector2(i * 250, 0);
            }
        }

        public static List<BaseNode> BuildCompleteNodeSequence(List<BaseNode> mainNodes, List<int> functionCallPositions, List<string> functionCallNames, Dictionary<string, GroupCallsNode> functionMap)
        {
            var completeSequence = new List<BaseNode>(mainNodes);

            // Находим StartNode и убеждаемся, что он первый
            var startNode = completeSequence.FirstOrDefault(n => n is SiphoinUnityHelpers.XNodeExtensions.BaseNodeInteraction &&
                                                                n.GetType().Name.Equals("StartNode", System.StringComparison.OrdinalIgnoreCase));

            if (startNode != null)
            {
                completeSequence.Remove(startNode);
                completeSequence.Insert(0, startNode);
            }

            // Вставляем GroupCallsNode в нужные позиции согласно именам вызываемых функций
            if (functionCallPositions != null && functionCallPositions.Count > 0 &&
                functionCallNames != null && functionCallPositions.Count == functionCallNames.Count)
            {
                // Создаем пары (позиция, имя функции) и сортируем по позиции в обратном порядке
                var callInfo = new List<(int position, string functionName)>();
                for (int i = 0; i < functionCallPositions.Count && i < functionCallNames.Count; i++)
                {
                    callInfo.Add((functionCallPositions[i], functionCallNames[i]));
                }

                // Сортируем по позиции в убывающем порядке, чтобы индексы не смещались при вставке
                var sortedCallInfo = callInfo.OrderByDescending(x => x.position).ToList();

                foreach (var (pos, functionName) in sortedCallInfo)
                {
                    if (functionMap.ContainsKey(functionName))
                    {
                        var groupNode = functionMap[functionName];
                        if (groupNode != null && pos < completeSequence.Count && pos >= 0)
                        {
                            completeSequence.Insert(pos, groupNode);
                        }
                    }
                }
            }
            else
            {
                // Если нет информации о позициях вызовов или именах, вставляем все GroupCallsNode в конец
                foreach (var groupNode in functionMap.Values)
                {
                    completeSequence.Add(groupNode);
                }
            }

            return completeSequence;
        }
    }
}