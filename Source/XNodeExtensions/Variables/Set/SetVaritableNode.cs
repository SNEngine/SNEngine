using UnityEngine;
using System.Linq;
using SNEngine.Graphs;
using System.Collections.Generic;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.Set
{
    public abstract class SetVariableNode<T> : BaseNodeInteraction
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private T _variable;
        [Input, SerializeField] private T _value;
        [HideInInspector, SerializeField] private string _targetGuid;
        [HideInInspector, SerializeField] private int _index;

        private T _editorOldValue;

        public string TargetGuid { get => _targetGuid; set => _targetGuid = value; }
        protected T Value { get => _value; set => _value = value; }

        public override void Execute()
        {
            var outputVariable = GetInputPort(nameof(_variable));
            var inputValue = GetInputPort(nameof(_value));
            var connectedVariables = outputVariable.GetConnections();

            object finalValue = _value;
            var connectedValue = inputValue.Connection;

            if (connectedValue != null)
            {
                finalValue = connectedValue.GetOutputValue();
            }

            if (connectedVariables.Count == 0 && !string.IsNullOrEmpty(_targetGuid))
            {
                VariableNode targetNode = null;

                if (graph is BaseGraph baseGraph)
                {
                    targetNode = baseGraph.GetNodeByGuid(_targetGuid) as VariableNode;
                }

                if (targetNode == null)
                {
                    targetNode = FindGlobalNode(_targetGuid);
                }

                if (targetNode is VariableNode<T> typedNode)
                {
                    SetTypedValue(typedNode, finalValue);
                }
                else if (targetNode is VariableCollectionNode<T> collectionNode)
                {
                    if (finalValue is T castValue)
                    {
                        collectionNode.SetValue(_index, castValue);
                    }
                }
            }
            else
            {
                foreach (var port in connectedVariables)
                {
                    var connectedVariable = port.node;

                    if (connectedVariable is VariableNode<T> VariableNode)
                    {
                        SetTypedValue(VariableNode, finalValue);
                    }

                    if (connectedVariable is VariableCollectionNode<T> collectionNode)
                    {
                        int index = RegexCollectionNode.GetIndex(port);
                        if (finalValue is T castValue)
                        {
                            collectionNode.SetValue(index, castValue);
                        }
                    }
                }
            }
        }

        protected virtual void OnSetTargetValueChanged(VariableNode<T> targetNode, T newValue) { }

#if UNITY_EDITOR
        protected override void OnEnable()
        {
            base.OnEnable();
            _editorOldValue = _value;
        }

        protected virtual void OnValidate()
        {
            if (!EqualityComparer<T>.Default.Equals(_editorOldValue, _value))
            {
                _editorOldValue = _value;
                OnSetTargetValueChanged(null, _value);
            }
        }
#endif

        private VariableNode FindGlobalNode(string guid)
        {
            var containers = Resources.LoadAll<VariableContainerGraph>("");
            foreach (var container in containers)
            {
                var node = container.nodes.OfType<VariableNode>().FirstOrDefault(n => n.GUID == guid);
                if (node != null) return node;
            }
            return null;
        }

        private void SetTypedValue(VariableNode<T> node, object value)
        {
            T castValue = default;
            bool success = false;

            if (value is T directValue)
            {
                castValue = directValue;
                success = true;
            }
            else
            {
                try
                {
                    castValue = (T)System.Convert.ChangeType(value, typeof(T));
                    success = true;
                }
                catch { }
            }

            if (success)
            {
                node.SetValue(castValue);
                OnSetTargetValueChanged(node, castValue);
            }
        }
    }
}