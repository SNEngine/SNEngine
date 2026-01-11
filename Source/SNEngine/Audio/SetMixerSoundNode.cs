using UnityEngine;
using UnityEngine.Audio;

namespace SNEngine.Audio
{
    public class SetMixerSoundNode : AudioNodeInteraction
    {
        [Input, SerializeField] private AudioMixerGroup _mixer;

        protected override void Interact(AudioObject input)
        {
            var value = GetInputValue<AudioMixerGroup>(nameof(_mixer), _mixer);
            input.Mixer = value;
        }
    }
}
