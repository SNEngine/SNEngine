using Cysharp.Threading.Tasks;
using UnityEngine;
using XNode;
using System;
using System.Collections.Generic;

namespace SiphoinUnityHelpers.XNodeExtensions.NodesControlExecutes
{
    public class IfNode : NodeControlExecute
    {
        private const float DURATION_SHOW_RESULT = 3.2f;
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private bool _condition;
        [Output, SerializeField] private NodePort _true;
        [Output, SerializeField] private NodePort _false;

        public override void Execute()
        {
            var condition = GetDataFromPort<bool>(nameof(_condition));

            string portName = condition ? nameof(_true) : nameof(_false);
            NodePort targetPort = GetOutputPort(portName);

            HighlightBranchRecursiveAsync(targetPort, condition).Forget();

            ExecuteNodesFromPort(targetPort).Forget();
        }

        private async UniTaskVoid HighlightBranchRecursiveAsync(NodePort startPort, bool condition)
        {
            Color color = condition ? Color.green : Color.red;

            HashSet<Node> branchNodes = new HashSet<Node>();
            branchNodes.Add(this);
            GetChildNodesRecursive(startPort, branchNodes);

            foreach (var node in branchNodes)
            {
                NodeHighlighter.HighlightNode(node, color);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(DURATION_SHOW_RESULT));

            foreach (var node in branchNodes)
            {
                NodeHighlighter.RemoveHighlight(node);
            }
        }

        private void GetChildNodesRecursive(NodePort port, HashSet<Node> visited)
        {
            if (port == null || !port.IsConnected) return;

            foreach (var connection in port.GetConnections())
            {
                Node nextNode = connection.node;
                if (nextNode != null && !visited.Contains(nextNode))
                {
                    visited.Add(nextNode);

                    foreach (NodePort output in nextNode.Outputs)
                    {
                        GetChildNodesRecursive(output, visited);
                    }
                }
            }
        }

        public bool NodeContainsOnBranch(BaseNodeInteraction node)
        {
            var portTrue = GetOutputPort(nameof(_true));
            var portFalse = GetOutputPort(nameof(_false));

            HashSet<Node> branchNodes = new HashSet<Node>();
            GetChildNodesRecursive(portTrue, branchNodes);
            GetChildNodesRecursive(portFalse, branchNodes);

            return branchNodes.Contains(node);
        }
    }
}