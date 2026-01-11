
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent.Scale
{
    public class SetScaleToTransformNode : SetWorldPropertyToTransformNode<Vector3>
    {
        protected override void SetCurrentProperty(Transform transform, Vector3 data)
        {
            SetScale(transform, data);
        }

        protected override void SetLocalProperty(Transform transform, Vector3 data)
        {
            SetScale(transform, data);
        }

        private void SetScale (Transform transform, Vector3 data)
        {
            transform.localScale = data;
        }
    }
}
