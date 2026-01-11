using SiphoinUnityHelpers.XNodeExtensions.Variables.DictionarySystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SNEngine.Graphs;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.DictionarySystem.Set
{
    public abstract class SetDictionaryVariableNode<TKey, TValue> : BaseNodeInteraction
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private DictionaryVariableNode<TKey, TValue> _dictionary;
        [SerializeField, HideInInspector] private string _guidVariable;
        [Input, SerializeField] private TKey _key;
        [Input, SerializeField] private TValue _value;

        public override void Execute()
        {
            TKey finalKey = GetInputValue(nameof(_key), _key);
            TValue finalValue = GetInputValue(nameof(_value), _value);

            DictionaryVariableNode<TKey, TValue> dictNode = null;
            var dictPort = GetInputPort(nameof(_dictionary));

            if (dictPort.IsConnected)
            {
                dictNode = dictPort.GetConnection(0).node as DictionaryVariableNode<TKey, TValue>;
            }
            else if (!string.IsNullOrEmpty(_guidVariable))
            {
                dictNode = FindVariableByGuid<DictionaryVariableNode<TKey, TValue>>(_guidVariable);
            }

            if (dictNode != null)
            {
                dictNode[finalKey] = finalValue;
                OnValueApplied(dictNode, finalKey, finalValue);
            }
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

        protected virtual void OnValueApplied(DictionaryVariableNode<TKey, TValue> node, TKey key, TValue value) { }
    }
}