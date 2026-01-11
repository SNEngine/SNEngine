using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.Finding.Tag
{
    public class FindGameObjectWithTagNode : FindWithTagNode<GameObject>
    {
        protected override GameObject ReturnResult(string tag)
        {
            return GameObject.FindGameObjectWithTag(tag);
        }
    }
}
