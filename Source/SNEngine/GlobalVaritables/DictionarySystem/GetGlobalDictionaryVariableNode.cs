using SiphoinUnityHelpers.XNodeExtensions;
using SiphoinUnityHelpers.XNodeExtensions.Variables.DictionarySystem;
using SNEngine.Services;
using System.Linq;
using UnityEngine;
using XNode;

namespace SNEngine.GlobalVariables.DictionarySystem
{
    public abstract class GetGlobalDictionaryVariableNode<TKey, TValue> : BaseNode
    {
        [SerializeField, HideInInspector] public string _guidVariable;
        [Input] public TKey _key;
        [Output] public TValue _result;

        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying || port.fieldName != nameof(_result)) return null;

            var service = NovelGame.Instance.GetService<VariablesContainerService>();
            if (service?.GlobalVariables == null) return null;

            string targetGuid = _guidVariable?.Trim();
            if (string.IsNullOrEmpty(targetGuid)) return default(TValue);

            var node = service.GlobalVariables.Values.FirstOrDefault(v =>
                v != null && v.GUID != null && v.GUID.Trim() == targetGuid);

            if (node is DictionaryVariableNode<TKey, TValue> dictNode)
            {
                TKey finalKey = GetInputValue(nameof(_key), _key);
                if (dictNode.TryGetValue(finalKey, out TValue value))
                {
                    return value;
                }
            }

            return default(TValue);
        }
    }
}