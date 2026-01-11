using static XNode.Node;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Math.Compare
{
    public class CompareStringsNode : CompareNode
    {
        [SerializeField, Input(connectionType = ConnectionType.Override)] private string _a;

        [SerializeField, Input(connectionType = ConnectionType.Override)] private string _b;

        [SerializeField] private CompareStringsType _type;

        protected override bool Compute()
        {
            var a = _a;

            var b = _b;

            var inputA = GetInputPort(nameof(_a));

            var inputB = GetInputPort(nameof(_b));

            if (inputA.Connection != null)
            {
                a = GetDataFromPort<string>(nameof(_a));
            }

            if (inputB.Connection != null)
            {
                b = GetDataFromPort<string>(nameof(_b));
            }

            return CompareHelper.Compare(a, b, _type);
        }
    }
}
