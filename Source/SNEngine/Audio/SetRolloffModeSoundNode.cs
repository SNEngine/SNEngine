using UnityEngine;

namespace SNEngine.Audio
{
    public class SetRolloffModeSoundNode : AudioNodeInteraction
    {
        [Input, SerializeField] private AudioRolloffMode _mode = AudioRolloffMode.Logarithmic;
        protected override void Interact(AudioObject input)
        {
            var value = GetInputValue<AudioRolloffMode>(nameof(_mode), _mode);
            input.RolloffMode = value;
        }
    }
}
