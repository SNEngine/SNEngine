using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent.EulerAngles
{
    public class GetEulerAnglesFromTransformNode : GetWorldPropertyFromTransformNode<Vector3>
    {
        protected override Vector3 GetCurrentProperty(Transform transform)
        {
            return transform.eulerAngles;
        }

        protected override Vector3 GetLocalProperty(Transform transform)
        {
            return transform.localEulerAngles;
        }
    }
}
