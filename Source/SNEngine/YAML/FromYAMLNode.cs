using Newtonsoft.Json;
using SiphoinUnityHelpers.XNodeExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XNode;
using static XNode.Node;
using Object = UnityEngine.Object;

namespace SNEngine.YAML
{
    public class FromYAMLNode : BaseNode 
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private string _yaml;

        [Output(ShowBackingValue.Never), SerializeField] private Object _result;

        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }

            var input = GetInputPort(nameof(_yaml));

            string yaml = _yaml;

            if (input.Connection != null)
            {
                yaml = (string)input.Connection.GetOutputValue();
            }
            SharpYaml.Serialization.Serializer serializer = new();
            return serializer.Deserialize(yaml);
        }
    }
}
