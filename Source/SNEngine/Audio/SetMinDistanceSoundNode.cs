using UnityEngine;

namespace SNEngine.Audio
{
    public class SetMinDistanceSoundNode : AudioNodeInteraction
    {
        [Input, SerializeField, Min(0f)] private float _minDistance = 1f;
        protected override void Interact(AudioObject input)
        {
            var value = GetInputValue<float>(nameof(_minDistance), _minDistance);
            input.MinDistance = value;
        }
    }
}
