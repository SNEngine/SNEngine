using Cysharp.Threading.Tasks;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Debugging;
using UnityEngine;

namespace SNEngine.Audio
{
    public abstract class AudioNodeInteractionAsync : AsyncNode
    {
        [Input(ShowBackingValue.Never), SerializeField] private AudioObject _input;

        public override void Execute()
        {
            base.Execute();
            var input = GetInputValue<AudioObject>(nameof(_input));
            if (!input)
            {
                NovelGameDebug.LogError($"invalid audio object input or input is null");
                return;
            }
            Interact(input).Forget();
        }

        protected abstract UniTask Interact(AudioObject input);
    }
}
