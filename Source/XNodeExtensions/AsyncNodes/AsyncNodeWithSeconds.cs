using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.AsyncNodes
{
    public abstract class AsyncNodeWithSeconds : AsyncNode
    {
        [Input(connectionType = ConnectionType.Override), SerializeField, Min(0)] private float _seconds;

        protected float GetSeconds()
        {
            float seconds = _seconds;

            var inputSeconds = GetInputPort(nameof(_seconds));

            if (inputSeconds.Connection != null)
            {
                seconds = GetDataFromPort<float>(nameof(_seconds));
            }

            return seconds;
        }
    }

}
