using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent.Scale
{
    public class GetScaleFromTransformNode : GetWorldPropertyFromTransformNode<Vector3>
    {
        protected override Vector3 GetCurrentProperty(Transform transform)
        {
            return transform.lossyScale;
        }

        protected override Vector3 GetLocalProperty(Transform transform)
        {
            return transform.localScale;
        }
    }
}
