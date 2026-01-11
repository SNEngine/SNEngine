using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent.Position
{
    public class GetPositionFromTransformNode : GetWorldPropertyFromTransformNode<Vector3>
    {
        protected override Vector3 GetCurrentProperty(Transform transform)
        {
            return transform.position;
        }

        protected override Vector3 GetLocalProperty(Transform transform)
        {
           return transform.localPosition;
        }
    }
}
