using UnityEngine;

namespace SNEngine.Audio
{
    public class SetMaxDistanceSoundNode : AudioNodeInteraction
    {
        [Input, SerializeField, Min(0.1f)] private float _maxDistance = 500f;
        protected override void Interact(AudioObject input)
        {
            var value = GetInputValue<float>(nameof(_maxDistance), _maxDistance);
            input.MaxDistance = value;
        }
    }
}
