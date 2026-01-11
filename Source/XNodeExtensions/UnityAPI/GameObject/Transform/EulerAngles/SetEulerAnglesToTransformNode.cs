
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent.EulerAngles
{
    public class SetEulerAnglesToTransformNode : SetWorldPropertyToTransformNode<Vector3>
    {
        protected override void SetCurrentProperty(Transform transform, Vector3 data)
        {
            transform.eulerAngles = data;
        }

        protected override void SetLocalProperty(Transform transform, Vector3 data)
        {
            transform.localEulerAngles = data;
        }
    }
}
