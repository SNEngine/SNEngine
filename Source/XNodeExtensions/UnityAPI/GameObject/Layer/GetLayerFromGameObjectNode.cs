using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.Layer
{
    public class GetLayerFromGameObjectNode : GetDataFromGameObjectNode<int>
    {
        protected override int GetData(GameObject gameObject)
        {
            return gameObject.layer;
        }
    }
}
