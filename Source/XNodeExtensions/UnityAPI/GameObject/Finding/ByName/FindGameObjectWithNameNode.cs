using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects.Finding.ByName
{
    public class FindGameObjectWithNameNode : FindWithNameNode<GameObject>
    {
        protected override GameObject ReturnResult(string name)
        {
            return GameObject.Find(name);
        }
    }
}
