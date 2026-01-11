using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent
{
    public abstract class GetDataFromTransformNode<TOutput> : BaseNode
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private Transform _transform;

        [Output(ShowBackingValue.Never), SerializeField] private TOutput _output;

        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }

            var transform = GetDataFromPort<Transform>(nameof(_transform));

            return GetData(transform);
        }
        protected abstract TOutput GetData(Transform transform);
    }
}
