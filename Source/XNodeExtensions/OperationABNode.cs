using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions
{
    public abstract class OperationABNode<T, TOutput> : BaseNode
    {
        [SerializeField, Input(connectionType = ConnectionType.Override)] private T _a;

        [SerializeField, Input(connectionType = ConnectionType.Override)] private T _b;

        [SerializeField, Output(ShowBackingValue.Never)] private TOutput _output;

        public override object GetValue(NodePort port)
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


            return Operation(a, b);
        }

        protected abstract T Operation(T a, T b);
    }
}
