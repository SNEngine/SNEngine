using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SNEngine.Audio
{
    public class FadeInSoundNode : AudioNodeInteractionAsync
    {
        [Input, SerializeField, Min(0f)] private float _duration = 1f;
        [Input, SerializeField, Range(0f, 1f)] private float _targetVolume = 1f;

        protected override async UniTask Interact(AudioObject input)
        {
            var duration = GetInputValue<float>(nameof(_duration), _duration);
            var targetVolume = GetInputValue(nameof(_targetVolume), _targetVolume);
            await input.FadeInAsync(duration, targetVolume);
            StopTask();
        }
    }
}
