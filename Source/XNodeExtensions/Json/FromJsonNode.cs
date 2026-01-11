using System;
using UnityEngine;
using XNode;
using Object = UnityEngine.Object;
using Newtonsoft.Json;
namespace SiphoinUnityHelpers.XNodeExtensions.Json
{
    public class FromJsonNode : BaseNode
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private string _json;

        [Output(ShowBackingValue.Never), SerializeField] private Object _result;

        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }

            var input = GetInputPort(nameof(_json));

            string json = _json;

            if (input.Connection != null)
            {
                json = (string)input.Connection.GetOutputValue();
            }

            return JsonConvert.DeserializeObject(json);
        }



    }
}
