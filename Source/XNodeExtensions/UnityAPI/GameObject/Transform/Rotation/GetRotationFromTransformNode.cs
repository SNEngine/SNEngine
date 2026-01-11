using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent.Rotation
{
    public class GetRotationFromTransformNode : GetWorldPropertyFromTransformNode<Quaternion>
    {
        protected override Quaternion GetCurrentProperty(Transform transform)
        {
            return transform.rotation;
        }

        protected override Quaternion GetLocalProperty(Transform transform)
        {
            return transform.localRotation;
        }
    }
}
