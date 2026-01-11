using UnityEngine;

namespace SNEngine.Audio
{
    public class SetPrioritySoundNode : AudioNodeInteraction
    {
        [Input, SerializeField, Range(0, 256)] private int _priority = 128;
        protected override void Interact(AudioObject input)
        {
            input.Priority = _priority;
        }
    }
}
