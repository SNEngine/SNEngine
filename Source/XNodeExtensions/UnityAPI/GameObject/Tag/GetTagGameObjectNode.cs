using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.Tag
{
    public class GetTagGameObjectNode : GetDataFromGameObjectNode<string>
    {
        protected override string GetData(GameObject gameObject)
        {
            return gameObject.tag;
        }
    }
}
