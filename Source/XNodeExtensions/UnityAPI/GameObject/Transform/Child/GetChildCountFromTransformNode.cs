using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent.Child
{
    public class GetChildCountFromTransformNode : GetDataFromTransformNode<int>
    {
        protected override int GetData(Transform transform)
        {
            return transform.childCount;
        }
    }
}
