using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    public abstract class OperationStringNode<TOutput> : BaseNode
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private string _string;

        [Output(ShowBackingValue.Never), SerializeField] private TOutput _output;

        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }
            var input = GetDataFromPort<string>(nameof(_string));

            return Operation(input);
        }

        protected abstract TOutput Operation(string input);
    }
}
