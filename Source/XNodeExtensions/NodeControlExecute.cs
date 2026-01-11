using Cysharp.Threading.Tasks;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SiphoinUnityHelpers.XNodeExtensions.Debugging;
using SiphoinUnityHelpers.XNodeExtensions.Extensions;
using SNEngine.AsyncNodes;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions
{
    [NodeTint("#593d6b")]
    public abstract class NodeControlExecute : BaseNodeInteraction, IIncludeWaitingNode
    {
        protected CancellationTokenSource _branchCts;
        private bool _isBranchWorking;

        public bool IsWorking => _isBranchWorking;

        public void SkipWait()
        {
            _branchCts?.Cancel();
        }

        protected async UniTask ExecuteNodesFromPort(NodePort port)
        {
            var connections = port.GetConnections();

            if (connections != null && connections.Count > 0)
            {
                _isBranchWorking = true;
                _branchCts = new CancellationTokenSource();

                List<UniTask> branchTasks = new List<UniTask>();

                foreach (var connect in connections)
                {
                    var node = connect.node as BaseNodeInteraction;
                    if (node != null)
                    {
                        branchTasks.Add(ExecuteAndHighlightBranch(node));
                    }
                }

                await UniTask.WhenAll(branchTasks);

                _branchCts?.Cancel();
                _branchCts = null;
                _isBranchWorking = false;
            }
        }

        private async UniTask ExecuteAndHighlightBranch(BaseNodeInteraction node)
        {
            if (_branchCts != null && _branchCts.IsCancellationRequested) return;

            NodeHighlighter.HighlightNode(node, Color.cyan);

            node.Execute();

            if (node is IIncludeWaitingNode waitingNode)
            {
                await XNodeExtensionsUniTask.WaitAsyncNode(waitingNode, _branchCts);
            }

            var exitPort = node.GetExitPort();
            if (exitPort != null && exitPort.IsConnected)
            {
                var nextNode = exitPort.Connection.node as BaseNodeInteraction;
                if (nextNode != null)
                {
                    await ExecuteAndHighlightBranch(nextNode);
                }
            }

            NodeHighlighter.RemoveHighlight(node);
        }
    }
}