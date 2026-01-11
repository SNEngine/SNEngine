using Cysharp.Threading.Tasks;
using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using System.Linq;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.NodesControlExecutes
{
    [NodeTint("#4b3359")]
    public class GroupCallsNode : NodeControlExecute
    {
        [SerializeField, ReadOnly(ReadOnlyMode.OnEditor)] private string _name;

        [Output, SerializeField] private NodePort _operations;
        public override void Execute()
        {
               var inputOperations = GetOutputPort(nameof(_operations));

                ExecuteNodesFromPort(inputOperations).Forget();

        }

        public bool NodeContainsOnOperation (BaseNodeInteraction node)
        {
            var inputOperations = GetOutputPort(nameof(_operations));

            var connections = inputOperations.GetConnections();

            if (connections is null)
            {
                return false;
            }

            return connections.Contains(node.GetEnterPort());
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            name = string.IsNullOrEmpty(_name) ? GetDefaultName() : $"{_name} ({GetDefaultName()})";
        }
#endif
    }
}
