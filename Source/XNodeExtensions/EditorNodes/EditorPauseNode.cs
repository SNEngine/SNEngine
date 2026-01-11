using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.EditorNodes
{
    public class EditorPauseNode : BaseNodeInteraction
    {
        public override void Execute()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Debug.Break();
            }
#endif
        }
    }
}