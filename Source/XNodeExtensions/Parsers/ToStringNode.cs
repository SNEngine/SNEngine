using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Parsers
{
    public class ToStringNode : BaseNode
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private NodePort _input;

        [Output(ShowBackingValue.Never), SerializeField] private string _output;
        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }
             return GetInputPort(nameof(_input)).Connection.GetOutputValue().ToString();
        }
    }
}
