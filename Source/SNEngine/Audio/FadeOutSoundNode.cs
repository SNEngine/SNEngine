using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SNEngine.Audio
{
    public class FadeOutSoundNode : AudioNodeInteractionAsync
    {
        [Input, SerializeField, Min(0f)] private float _duration = 1f;
        protected override async UniTask Interact(AudioObject input)
        {
            var duration = GetInputValue<float>(nameof(_duration), _duration);
            await input.FadeOutAsync(duration);
            StopTask();
        }
    }
}
