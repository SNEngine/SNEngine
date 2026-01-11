using UnityEngine;

namespace SNEngine.Audio
{
    public class SetPitchSoundNode : AudioNodeInteraction
    {
        [Input, SerializeField, Range(-3f, 3f)] private float _pitch = 1f;
        protected override void Interact(AudioObject input)
        {
            var value = GetInputValue<float>(nameof(_pitch), _pitch);
            input.Pitch = value;
        }
    }
}
