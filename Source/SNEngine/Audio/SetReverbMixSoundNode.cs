using UnityEngine;

namespace SNEngine.Audio
{
    public class SetReverbMixSoundNode : AudioNodeInteraction
    {
        [Input, SerializeField, Range(0f, 1.1f)] private float _mix = 1f;
        protected override void Interact(AudioObject input)
        {
            var value = GetInputValue<float>(nameof(_mix), _mix);
            input.ReverbZoneMix = value;
        }
    }
}
