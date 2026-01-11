using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.TimeSystem
{
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Unity API/Time/Get Time Scale")]
    public class GetTimeScaleNode : GetTimeValueNode
    {
        protected override float GetValue()
        {
            return Time.timeScale;
        }
    }
}
