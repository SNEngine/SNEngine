using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using XNode;
using Debug = UnityEngine.Debug;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects
{
    [NodeWidth(260)]
    public class GetComponextNode : BaseNode
    {

        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private GameObject _gameObject;

        [Space(5)]

        [Input(connectionType = ConnectionType.Override), SerializeField] private string _type = "TypeName";

        [Space(5)]

        [Output(connectionType = ConnectionType.Override), SerializeField] private Component _outputComponent;

        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }

            if (_outputComponent != null)
            {
                return _outputComponent;
            }

            var gameObject = GetDataFromPort<GameObject>(nameof(_gameObject));

            string typeString = _type;

            if (GetInputPort(nameof(_type)).Connection != null)
            {
                typeString = GetInputValue<string>(nameof(_type));
            }

            Type type = FinderType.Find(typeString);

            if (!gameObject.TryGetComponent(type, out _outputComponent))
            {
                throw new InvalidOperationException($"component with type {type.Name} not found on GameObject {gameObject.name}");
            }

            else
            {
                return _outputComponent;
            }



        }
    }

}
