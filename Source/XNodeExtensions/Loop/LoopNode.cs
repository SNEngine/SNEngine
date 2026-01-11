using SiphoinUnityHelpers.XNodeExtensions.Interfaces;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Loop
{
    [NodeTint("#593d6b")]
    public abstract class LoopNode : BaseNodeInteraction, ILoopNode
    {
        [Space]
        [Output, SerializeField] private LoopPort _loop;

        protected void CallLoop()
        {
            var port = GetOutputPort(nameof(_loop));

            if (port.ConnectionCount > 0)
            {
                var connections = port.GetConnections();

                foreach (var item in connections)
                {
                    var node = item.node as BaseNodeInteraction;

                    if (node != null)
                    {
                        node.Execute();
                    }
                }
            }
        }

        public bool NodeContainsOnLoop(BaseNodeInteraction node)
        {
            var port = GetOutputPort(nameof(_loop));

            if (port.ConnectionCount > 0)
            {
                var connections = port.GetConnections();

                return connections.Contains(node.GetEnterPort());
            }

            return false;
        }
    }
}