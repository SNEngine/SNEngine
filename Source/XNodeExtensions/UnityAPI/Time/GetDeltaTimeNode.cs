using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.TimeSystem
{
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Unity API/Time/Get deltaTime")]
    public class GetDeltaTimeNode : GetTimeValueNode
    {
        [SerializeField] private DeltaTimeType _property;
        protected override float GetValue()
        {
            switch (_property)
            {
                case DeltaTimeType.Delta:
                    return Time.deltaTime;
                case DeltaTimeType.Fixed:
                    return Time.fixedDeltaTime;
                case DeltaTimeType.Capture:
                    return Time.captureDeltaTime;
                case DeltaTimeType.Unscaled:
                    return Time.unscaledDeltaTime;
                case DeltaTimeType.FixedUnscaled:
                    return Time.fixedUnscaledDeltaTime;
                case DeltaTimeType.Maximum:
                    return Time.maximumDeltaTime;
                case DeltaTimeType.Smooth:
                    return Time.smoothDeltaTime;
                case DeltaTimeType.MaximumParticleDeltaTime:
                    return Time.maximumParticleDeltaTime;
            }

            return float.NaN;
        }
    }
}
