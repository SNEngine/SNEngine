using UnityEngine;

namespace SNEngine.Audio
{
    public class SetSpreadSoundNode : AudioNodeInteraction
    {
        [Input, SerializeField, Range(0f, 360f)] private float _spread = 0f;
        protected override void Interact(AudioObject input)
        {
            var value = GetInputValue<float>(nameof(_spread), _spread);
            input.Spread = value;
        }
    }
}
