using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.GameObjects
{
    internal class DestroyImmediateGameObjectNode : DestroyNode
    {
        protected override void DestroyGameObject(GameObject gameObject)
        {
            DestroyImmediate(gameObject);
        }
    }
}
