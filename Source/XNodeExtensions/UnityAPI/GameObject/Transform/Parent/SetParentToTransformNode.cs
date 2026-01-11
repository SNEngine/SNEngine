using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent.Parent

{
    public class SetParentToTransformNode : SetDataToTransformNode<Transform>
    {
        protected override void SetData(Transform transform, Transform data)
        {
            transform.SetParent(data);
        }
    }
}
