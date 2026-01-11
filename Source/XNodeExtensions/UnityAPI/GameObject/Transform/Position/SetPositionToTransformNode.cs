
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent.Position
{
    public class SetPositionToTransformNode : SetWorldPropertyToTransformNode<Vector3>
    {
        protected override void SetCurrentProperty(Transform transform, Vector3 data)
        {
            transform.position = data;
        }

        protected override void SetLocalProperty(Transform transform, Vector3 data)
        {
            transform.localPosition = data;
        }
    }
}
