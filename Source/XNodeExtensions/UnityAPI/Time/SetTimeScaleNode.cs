
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.TimeSystem
{
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Unity API/Time/Set Time Scale")]
    public class SetTimeScaleNode : BaseNodeInteraction
    {
        [Input(connectionType = ConnectionType.Override), Min(0), SerializeField] private float _timeScale;

        public override void Execute()
        {
            var timeScale = _timeScale;

            var input = GetInputPort(nameof(_timeScale));

            if (input.Connection != null)
            {
                timeScale = GetDataFromPort<float>(nameof(_timeScale));
            }

            Time.timeScale = timeScale;
        }
    }
}
