using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.Name
{
    public class GetNameFromGameObjectNode : GetDataFromGameObjectNode<string>
    {
        protected override string GetData(GameObject gameObject)
        {
             return gameObject.name;
        }
    }
}
