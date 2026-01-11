using Cysharp.Threading.Tasks;
using SiphoinUnityHelpers.XNodeExtensions.Debugging;
using SiphoinUnityHelpers.XNodeExtensions.Extensions;
using SNEngine.AsyncNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions
{
    public abstract class BaseGraph : NodeGraph
    {
        [SerializeField]
        private string _guid;

        private NodeQueue _queue;

        private IDictionary<string, VariableNode> _variables;
        public event Action OnStartExecute;
        public event Action OnEndExecute;

        public event Action<BaseNode> OnNextNode;
        new private List<Node> nodes => base.nodes;

        public bool IsPaused { get; private set; }

        public bool IsRunning => _queue is null ? false : !_queue.IsEnding;

        protected NodeQueue Queue => _queue;

        public IDictionary<string, VariableNode> Variables => _variables;

        public IReadOnlyDictionary<string, BaseNode> AllNodes
        {
            get
            {
                Dictionary<string, BaseNode> data = new();
                var baseNodes = nodes.OfType<BaseNode>();
                foreach (var node in baseNodes)
                {
                    var key = node.GUID;
                    data.Add(key, node);
                }

                return data;
            }
        }

        public string GUID
        {
            get
            {
#if UNITY_EDITOR
                RegenerateGUID();
#endif
                return _guid;
            }
        }
#if UNITY_EDITOR
        private void RegenerateGUID()
        {
            if (string.IsNullOrEmpty(_guid))
                ResetGUID();
        }

        private void ResetGUID()
        {
            _guid = Guid.NewGuid().ToShortGUID();
        }

        private void Awake()
        {
            RegenerateGUID();
        }

        private void FixDuplicateGUIDs()
        {
            HashSet<string> guids = new HashSet<string>();
            List<BaseNode> baseNodes = nodes.OfType<BaseNode>().ToList();

            foreach (var node in baseNodes)
            {
                if (guids.Contains(node.GUID))
                {
                    typeof(BaseNode)
                        .GetMethod("ResetGuid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .Invoke(node, null);
                }
                guids.Add(node.GUID);
            }
        }
#endif

        public virtual void Execute ()
        {
#if UNITY_EDITOR
            FixDuplicateGUIDs();
#endif
            var queue = new List<BaseNodeInteraction>();
            var normalizeNodes = TopologicalSortInteractionNodes();
            BuidVariableNodes();

            for (int i = 0; normalizeNodes.Count > i; i++)
            {
                var node = normalizeNodes[i];
                queue.Add(node);
            }

            _queue = new NodeQueue(this, queue);

            ExecuteProcess().Forget();
            OnStartExecute?.Invoke();

        }

        protected void BuidVariableNodes()
        {
            if (_variables is null)
            {
                Dictionary<string, VariableNode> nodes = new Dictionary<string, VariableNode>();

                foreach (var node in this.nodes)
                {
                    if (node is VariableNode)
                    {
                        VariableNode VariableNode = node as VariableNode;
                        nodes.Add(VariableNode.Name, VariableNode);
                    }
                }

                _variables = nodes;
            }
        }

        public T GetValueFromVariable<T>(string name)
        {
            var node = _variables[name];

            if (node is null)
            {
                throw new NullReferenceException($"Variable Node with name not found");
            }

            var value = node.GetCurrentValue();

            if (value.GetType() != typeof(T))
            {
                throw new InvalidCastException($"Variable node {node.Name} have type {value.GetType()}. Argument type {typeof(T)}");
            }

            return (T)value;
        }

        public virtual void Continue ()
        {
            IsPaused = true;
        }

        public virtual void Pause ()
        {
            IsPaused = false;
        }

        public virtual void Stop ()
        {
            End();

            _queue.Exit();       
        }

        private List<BaseNodeInteraction> TopologicalSortInteractionNodes()
        {
            List<BaseNodeInteraction> executableNodes = nodes
                .OfType<BaseNodeInteraction>()
                .ToList();

            var allInteractionNodes = executableNodes
                .ToDictionary(node => node, node => 0);

            foreach (var node in executableNodes)
            {
                foreach (var outputPort in node.Ports.Where(p => p.IsOutput && p.IsConnected))
                {
                    foreach (var connection in outputPort.GetConnections())
                    {
                        if (connection.node is BaseNodeInteraction nextNode)
                        {
                            if (allInteractionNodes.ContainsKey(nextNode))
                            {
                                allInteractionNodes[nextNode]++;
                            }
                        }
                    }
                }
            }

            var queue = new Queue<BaseNodeInteraction>(
                allInteractionNodes.Where(pair => pair.Value == 0).Select(pair => pair.Key)
            );

            var sortedList = new List<BaseNodeInteraction>();

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();

                if (!currentNode.IsControlledAnotherNode)
                {
                    sortedList.Add(currentNode);
                }
                foreach (var outputPort in currentNode.Ports.Where(p => p.IsOutput && p.IsConnected))
                {
                    foreach (var connection in outputPort.GetConnections())
                    {
                        if (connection.node is BaseNodeInteraction nextNode)
                        {
                            if (allInteractionNodes.ContainsKey(nextNode))
                            {
                                allInteractionNodes[nextNode]--;

                                if (allInteractionNodes[nextNode] == 0)
                                {
                                    queue.Enqueue(nextNode);
                                }
                            }
                        }
                    }
                }
            }

            return sortedList;
        }

        public virtual void JumptToNode(string targetGuid)
    {
#if UNITY_EDITOR
            FixDuplicateGUIDs();
#endif
            BuidVariableNodes();

        var allInteractionNodes = nodes
            .OfType<BaseNodeInteraction>()
            .ToList();

            allInteractionNodes = TopologicalSortInteractionNodes();

        int targetIndex = allInteractionNodes.FindIndex(node => node.GUID == targetGuid);

        List<BaseNodeInteraction> filteredNodes;

        if (targetIndex != -1)
        {

            filteredNodes = allInteractionNodes
                .Select((node, index) => new { Node = node, Index = index })
                .Where(item =>
                {
                    if (item.Index < targetIndex)
                    {
                        if (item.Node is IIncludeWaitingNode waitNode)
                        {
                            waitNode.SkipWait();
                        }

                        return item.Node.CanSkip() == false;
                    }
                    else
                    {
                        return true;
                    }
                })
                .Select(item => item.Node)
                .ToList();
        }
        else
        {
            filteredNodes = new List<BaseNodeInteraction>();
        }

        _queue = new NodeQueue(this, filteredNodes);

        ExecuteProcess().Forget();
    }





        public BaseNode GetNodeByGuid(string guid)
        {
            return nodes.OfType<BaseNode>().FirstOrDefault(n => n.GUID == guid);
        }

        private async UniTask ExecuteProcess ()
        {
            _queue.OnEnd += End;

            for (int i = 0; i < _queue.Count; i++)
            {
                await UniTask.WaitUntil(() => !IsPaused);

                var node = await _queue.Next();

                if (node is null)
                {
                    break;
                }

                OnNextNode?.Invoke(node);

               
            }
        }

        private void End()
        {
            _queue.OnEnd -= End;

            OnEndExecute?.Invoke();

            ResetVariables();

            XNodeExtensionsDebug.Log($"graph {name} end execute");
        }

        private void ResetVariables()
        {
            foreach (var node in nodes.Where(node => node is VariableNode))
            {
                VariableNode VariableNode = node as VariableNode;

                VariableNode.ResetValue();
            }
        }
    }
}
