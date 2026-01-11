using UnityEngine;

namespace SNEngine.Audio
{
    public class PlayScheduledSoundNode : AudioNodeInteraction
    {
        [Input, SerializeField, Min(0.0f)] private double _time;

        protected override void Interact(AudioObject input)
        {
            var value = GetInputValue<double>(nameof(_time), _time);
            input.PlayScheduled(value);
        }
    }
}
