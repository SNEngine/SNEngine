using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent.Child
{
    internal class GetChildsFromTransformNode : BaseNode
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private Transform _transform;

        [Output(ShowBackingValue.Never), SerializeField] private NodePortEnumerable _enumerable;

        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }

            var transform = GetDataFromPort<Transform>(nameof(_transform));

            List<GameObject> childrens = new List<GameObject>();

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).gameObject;

                childrens.Add(child);
            }

            return childrens;
        }
    }
}
