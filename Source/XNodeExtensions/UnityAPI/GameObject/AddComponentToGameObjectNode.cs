using System;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects
{
    public class AddComponentToGameObjectNode : SetDataToGameObjectNode
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private string _typeName;
        public override void SetData(GameObject gameObject)
        {
            string typeName = _typeName;

            var input = GetInputPort(nameof(_typeName));

            if (input.Connection != null)
            {
                typeName = GetDataFromPort<string>(nameof(_typeName));
            }

            Type type = FinderType.Find(typeName);

            gameObject.AddComponent(type);
        }
    }
}
