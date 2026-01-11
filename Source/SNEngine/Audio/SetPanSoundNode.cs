using UnityEngine;

namespace SNEngine.Audio
{
    public class SetPanSoundNode : AudioNodeInteraction
    {
        [Input, SerializeField, Range(-1f, 1f)] private float _pan = 0f;
        protected override void Interact(AudioObject input)
        {
            var value = GetInputValue<float>(nameof(_pan), _pan);
            input.PanStereo = value;
        }
    }
}
