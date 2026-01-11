using UnityEngine;

namespace SNEngine.Audio
{
    public class SetLoopSoundNode : AudioNodeInteraction
    {
        [Input, SerializeField] private bool _loop;
        protected override void Interact(AudioObject input)
        {
            var value = GetInputValue<bool>(nameof(_loop), _loop);
            input.Loop = value;
        }
    }

}
