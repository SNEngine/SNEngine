using UnityEngine;
using System.Linq;
using SNEngine.Graphs;
using System.Collections;
using XNode;
using SiphoinUnityHelpers.XNodeExtensions.Debugging;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.Collection
{
    public class RemoveFromCollectionNode : BaseNodeInteraction
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField]
        private object _collection;

        [Input, SerializeField]
        private int _index;

        [HideInInspector, SerializeField]
        private string _targetGuid;

        public override void Execute()
        {
            var port = GetInputPort(nameof(_collection));
            var indexPort = GetInputPort(nameof(_index));

            int finalIndex = indexPort.IsConnected ? (int)indexPort.GetOutputValue() : _index;

            if (!port.IsConnected && !string.IsNullOrEmpty(_targetGuid))
            {
                var targetNode = FindNode(_targetGuid);
                if (targetNode is IList list)
                {
                    Remove(list, finalIndex);
                }
            }
            else
            {
                foreach (var connection in port.GetConnections())
                {
                    if (connection.node is IList list)
                    {
                        Remove(list, finalIndex);
                    }
                }
            }
        }

        private void Remove(IList list, int index)
        {
            try
            {
                list.RemoveAt(index);
            }
            catch (System.Exception ex)
            {
                XNodeExtensionsDebug.LogError($"Error remove item from collection: {ex.Message}");
            }
        }

        private VariableNode FindNode(string guid)
        {
            if (graph is BaseGraph baseGraph)
            {
                var node = baseGraph.GetNodeByGuid(guid) as VariableNode;
                if (node != null) return node;
            }

            return Resources.LoadAll<VariableContainerGraph>("")
                .SelectMany(g => g.nodes)
                .OfType<VariableNode>()
                .FirstOrDefault(n => n.GUID == guid);
        }
    }
}