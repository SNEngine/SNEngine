using Cysharp.Threading.Tasks;
using UnityEngine;
using XNode;
using System;
using System.Collections.Generic;

namespace SiphoinUnityHelpers.XNodeExtensions.NodesControlExecutes.Switch
{
    public abstract class SwitchNode<T> : NodeControlExecute
    {
        private const float DURATION_SHOW_RESULT = 3.2f;

        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField]
        protected T _value;

        [SerializeField]
        protected List<T> _cases = new List<T>();

        [Output, SerializeField]
        protected NodePort _default;

        public override void Execute()
        {
            T val = GetDataFromPort<T>(nameof(_value));
            NodePort targetPort = GetOutputPort(nameof(_default));

            for (int i = 0; i < _cases.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(val, _cases[i]))
                {
                    targetPort = GetOutputPort(GetPortName(i));
                    break;
                }
            }

            if (targetPort != null)
            {
                HighlightBranchRecursiveAsync(targetPort).Forget();
                ExecuteNodesFromPort(targetPort).Forget();
            }
        }

        protected virtual string GetPortName(int index) => "case_" + index;

        private async UniTaskVoid HighlightBranchRecursiveAsync(NodePort startPort)
        {
            Color color = Color.cyan;
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
    }
}