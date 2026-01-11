using SiphoinUnityHelpers.XNodeExtensions;
using UnityEngine;
using XNode;
using Object = UnityEngine.Object;

namespace SNEngine.YAML
{
    public class ToYAMLNode : BaseNode
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private Object _targetObject;

        [Output(ShowBackingValue.Never), SerializeField] private string _yaml;

        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }

            var inputTarget = GetInputPort(nameof(_targetObject));

            var value = inputTarget.Connection.GetOutputValue();

            SharpYaml.Serialization.Serializer serializer = new();
            return serializer.Serialize(value);
        }
    }
}
