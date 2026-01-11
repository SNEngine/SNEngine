
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent.Rotation
{
    public class SetRotationToTransformNode : SetWorldPropertyToTransformNode<Quaternion>
    {
        protected override void SetCurrentProperty(Transform transform, Quaternion data)
        {
            transform.rotation = data;
        }

        protected override void SetLocalProperty(Transform transform, Quaternion data)
        {
            transform.localRotation = data;
        }
    }
}
