using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent.Parent
{
    public class UnpinParentInTransformNode : BaseNodeInteraction
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private Transform _transform;
        public override void Execute()
        {
            var transform = GetDataFromPort<Transform>(nameof(_transform));

            transform.SetParent(null);
        }
    }
}
