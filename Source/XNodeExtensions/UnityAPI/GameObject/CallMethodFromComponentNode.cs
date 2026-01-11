using System;
using System.Reflection;
using UnityEngine;
using XNode;
using Object = UnityEngine.Object;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects
{
    public class CallMethodFromComponentNode : BaseNodeInteraction
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private string _methodName;

        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private Component _component;

        [Input(ShowBackingValue.Never), SerializeField] private Object _params;

        [SerializeField] private BindingFlags _bindingFlags = BindingFlags.InvokeMethod;

        [Output(ShowBackingValue.Never), SerializeField] private Object _output;

        private object _outputData;

        public override void Execute()
        {
           Component component = GetInputValue<Component>(nameof(_component));

            string methodName = _methodName;

            string inputMethodName = GetDataFromPort<string>(nameof(_methodName));

            if (inputMethodName != null)
            {
                methodName = inputMethodName;
            }

            var inputParams = GetInputPort(nameof(_params));

            object[] paramsArray = null;

            if (inputParams.Connection != null)
            {
                var connections = inputParams.GetConnections();

                paramsArray = new object[connections.Count];

                for (int i = 0; i < connections.Count; i++)
                {
                    var connection = connections[i];

                    paramsArray[i] = connection.GetOutputValue();
                }

            }

            Type type = component.GetType();

            _outputData = type.InvokeMember(methodName, _bindingFlags, null, component, paramsArray);
        }

        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }

            return _outputData;
        }
    }
}
