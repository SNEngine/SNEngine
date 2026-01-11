using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.TimeSystem
{
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Unity API/Time/Reset Time Scale")]
    public class ResetTimeScaleNode : BaseNodeInteraction
    {
        public override void Execute()
        {
            Time.timeScale = 1;
        }
    }
}
