using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.Finding.Tag
{
    public class FindGameObjectsWithTagNode : FindWithTagNode<GameObject[]>
    {
        protected override GameObject[] ReturnResult(string tag)
        {
            return GameObject.FindGameObjectsWithTag(tag);
        }
    }
}
