using UnityEngine;

namespace SNEngine.Audio
{
    public class SetDopplerLevelSoundNode : AudioNodeInteraction
    {
        [Input, SerializeField, Min(0f)] private float _doppler = 1f;
        protected override void Interact(AudioObject input)
        {
            var value = GetInputValue<float>(nameof(_doppler), _doppler);
            input.DopplerLevel = value;
        }
    }
}
