using SiphoinUnityHelpers.Exceptions;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.NodesControlExecutes
{
    public class TryCatchNode : NodeControlExecute
    {
        [Space]

        [Output, SerializeField] private NodePort _try;

        [Output, SerializeField] private NodePort _catch;

        [Output, SerializeField] private NodePort _finally;

        [Space]

        [Output, SerializeField] private NodeException _outputException;

        private NodeException _exception;

        public override void Execute()
        {

            ExecuteNodes();
            
        }

        private void ExecuteNodes()
        {
            var connections = GetExitPort().GetConnections();

            if (connections != null)
            {
                    foreach (var item in connections)
                    {
                        var node = item.node as BaseNode;

                        try
                        {
                            node.Execute();
                        }
                        catch (System.Exception e)
                        {
                            NodeException nodeException = new NodeException(e.Message, node);

                            _exception = nodeException;

                            foreach (var itemOnCatch in GetOutputPort(nameof(_catch)).GetConnections())
                            {
                                var nodeCatch = itemOnCatch.node as BaseNode;



                                nodeCatch.Execute();
                            }
                        }
                    

                    
                    }


                }
                
            }
        

        public override object GetValue(NodePort port)
        {
            return _exception;
        }
    }
}
