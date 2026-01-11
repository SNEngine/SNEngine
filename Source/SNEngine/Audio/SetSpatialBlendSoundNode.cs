using UnityEngine;

namespace SNEngine.Audio
{
    public class SetSpatialBlendSoundNode : AudioNodeInteraction
    {
        [Input, SerializeField, Range(0f, 1f)] private float _blend = 1f;
        protected override void Interact(AudioObject input)
        {
            var value = GetInputValue<float>(nameof(_blend), _blend);
            input.SpatialBlend = value;
        }
    }
}
