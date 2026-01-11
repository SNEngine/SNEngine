using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.Debugging;
using SNEngine.Graphs;
using SNEngine.Services;
using System.Linq;
using UnityEngine;
using XNode;

namespace SNEngine.GlobalVariables
{
    public abstract class GetGlobalVariableNode<T> : BaseNode
    {
        [SerializeField, HideInInspector] private string _guidVariable;
        [Output] private T _result;

        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying || port.fieldName != "_result") return null;

            var service = NovelGame.Instance.GetService<VariablesContainerService>();
            if (service?.GlobalVariables == null) return null;

            string targetGuid = _guidVariable?.Trim();
            if (string.IsNullOrEmpty(targetGuid)) return default(T);

            var VariableNode = service.GlobalVariables.Values.FirstOrDefault(v =>
                v != null && v.GUID != null && v.GUID.Trim() == targetGuid);

            return VariableNode != null ? VariableNode.GetCurrentValue() : default(T);
        }
    }
}