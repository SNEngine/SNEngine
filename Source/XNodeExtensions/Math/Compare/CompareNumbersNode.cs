using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Math.Compare
{
    public abstract class CompareNumbersNode<T> : CompareNode
    {
        [SerializeField, Input(connectionType = ConnectionType.Override)] private T _a;

        [SerializeField, Input(connectionType = ConnectionType.Override)] private T _b;

        [SerializeField] private CompareType _type;

        protected override bool Compute()
        {
            var a = _a;

            var b = _b;

            var inputA = GetInputPort(nameof(_a));

            var inputB = GetInputPort(nameof(_b));

            if (inputA.Connection != null)
            {
                a = GetDataFromPort<T>(nameof(_a));
            }

            if (inputB.Connection != null)
            {
                b = GetDataFromPort<T>(nameof(_b));
            }

            return CompareHelper.Compare(a, b, _type);
        }

    }
}
