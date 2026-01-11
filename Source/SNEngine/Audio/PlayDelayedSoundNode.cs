using System;
using UnityEngine;

namespace SNEngine.Audio
{
    public class PlayDelayedSoundNode : AudioNodeInteraction
    {
        [Input, SerializeField, Min(0f)] private float _delay = 0.1f;

        protected override void Interact(AudioObject input)
        {
            var value = GetInputValue<float>(nameof(_delay), _delay);
            input.PlayDelayed(value);
        }
    }
}
