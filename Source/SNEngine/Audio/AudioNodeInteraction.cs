using SNEngine.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace SNEngine.Audio
{
    public abstract class AudioNodeInteraction : AudioNode
    {
        [Input(ShowBackingValue.Never), SerializeField] private AudioObject _input;

        public override void Execute()
        {
            var input = GetInputValue<AudioObject>(nameof(_input));
            Interact(input);
        }

        protected abstract void Interact(AudioObject input);

    }
}
