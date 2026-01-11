using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Math.Compare
{
    public class CompareBooleansNode : CompareNode
    {
        [SerializeField, Input(connectionType = ConnectionType.Override)] private bool _a;

        [SerializeField, Input(connectionType = ConnectionType.Override)] private bool _b;

        [SerializeField] private CompareBoolsType _type;

        protected override bool Compute()
        {
            var a = _a;

            var b = _b;

            var inputA = GetInputPort(nameof(_a));

            var inputB = GetInputPort(nameof(_b));

            if (inputA.Connection != null)
            {
                a = GetDataFromPort<bool>(nameof(_a));
            }

            if (inputB.Connection != null)
            {
                b = GetDataFromPort<bool>(nameof(_b));
            }

            return CompareHelper.Compare(a, b, _type);
        }
    }
}
