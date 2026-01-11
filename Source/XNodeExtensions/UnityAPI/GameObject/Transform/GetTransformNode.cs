using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.TransformComponent
{
    public class GetTransformNode : GetDataFromGameObjectNode<Transform>
    {
        protected override Transform GetData(GameObject gameObject)
        {
            return gameObject.transform;
        }
    }
}
