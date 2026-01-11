using System;
using UnityEngine;
using XNode;
using Object = UnityEngine.Object;
using Newtonsoft.Json;
namespace SiphoinUnityHelpers.XNodeExtensions.Json
{
    public class ToJsonNode : BaseNode
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private Object _targetObject;

        [Output(ShowBackingValue.Never), SerializeField] private string _json;

        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
               return base.GetValue(port);
            }

            var inputTarget = GetInputPort(nameof(_targetObject));

            var value = inputTarget.Connection.GetOutputValue();

            return JsonConvert.SerializeObject(value);
        }



    }
}
