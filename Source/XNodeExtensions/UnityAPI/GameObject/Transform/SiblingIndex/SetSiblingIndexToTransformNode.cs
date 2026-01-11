using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent.SiblingIndex
{
    public class SetSiblingIndexToTransformNode : SetDataToTransformNode<int>
    {
        protected override void SetData(Transform transform, int data)
        {
             transform.SetSiblingIndex(data);
        }
    }
}
