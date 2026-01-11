using UnityEngine;
using XNode;
using System.Linq;
using SNEngine.Graphs;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.DictionarySystem.Get
{
    public abstract class GetDictionaryVariableNode<TKey, TValue> : BaseNode
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] public DictionaryVariableNode<TKey, TValue> _dictionary;
        [SerializeField, HideInInspector] private string _guidVariable;
        [Input] public TKey _key;
        [Output] public TValue _value;

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == nameof(_value))
            {
                DictionaryVariableNode<TKey, TValue> dictNode = GetInputValue(nameof(_dictionary), _dictionary);

                if (dictNode == null && !string.IsNullOrEmpty(_guidVariable))
                {
                    dictNode = FindVariableByGuid<DictionaryVariableNode<TKey, TValue>>(_guidVariable);
                }

                TKey key = GetInputValue(nameof(_key), _key);

                if (dictNode != null && key != null)
                {
                    if (dictNode.TryGetValue(key, out TValue result))
                    {
                        return result;
                    }
                }
                return default(TValue);
            }
            return null;
        }

        protected T FindVariableByGuid<T>(string guid) where T : class
        {
            if (graph is BaseGraph baseGraph)
            {
                var localNode = baseGraph.GetNodeByGuid(guid) as T;
                if (localNode != null) return localNode;
            }

            var containers = Resources.LoadAll<VariableContainerGraph>("");
            foreach (var container in containers)
            {
                var node = container.nodes.OfType<T>().FirstOrDefault(n => (n as VariableNode)?.GUID == guid);
                if (node != null) return node;
            }
            return null;
        }
    }
}