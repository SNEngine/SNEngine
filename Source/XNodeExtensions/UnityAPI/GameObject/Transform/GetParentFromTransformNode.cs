
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent
{
    public class GetParentFromTransformNode : GetDataFromTransformNode<Transform>
    {
        protected override Transform GetData(Transform transform)
        {
            return transform.parent;
        }
    }
}
