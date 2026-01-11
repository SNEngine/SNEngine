using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent.SiblingIndex
{
    public class GetSiblingIndexFromTransformNode : GetDataFromTransformNode<int>
    {
        protected override int GetData(Transform transform)
        {
            return transform.GetSiblingIndex();
        }
    }
}
