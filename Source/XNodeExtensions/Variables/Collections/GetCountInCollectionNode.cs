using System.Collections;
using System.Linq;
using UnityEngine;
using XNode;
using SNEngine.Graphs;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.Collection
{
    public class GetCountInCollectionNode : BaseNode
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField]
        private object _collection;

        [Output(ShowBackingValue.Always), SerializeField]
        private int _count;

        [HideInInspector, SerializeField]
        private string _targetGuid;

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == nameof(_count))
            {
                return GetCollectionCount();
            }
            return null;
        }

        private int GetCollectionCount()
        {
            var port = GetInputPort(nameof(_collection));

            if (!port.IsConnected && !string.IsNullOrEmpty(_targetGuid))
            {
                var targetNode = FindNode(_targetGuid);
                if (targetNode is IList collection)
                {
                    return collection.Count;
                }
            }
            else
            {
                foreach (var connection in port.GetConnections())
                {
                    if (connection.node is IList collection)
                    {
                        return collection.Count;
                    }
                }
            }

            return 0;
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