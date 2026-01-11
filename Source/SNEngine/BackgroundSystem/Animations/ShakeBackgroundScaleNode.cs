using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Services;
using UnityEngine;

namespace SNEngine.BackgroundSystem.Animations
{
    public class ShakeBackgroundScaleNode : AsyncNode
    {
        [Input(ShowBackingValue.Unconnected), SerializeField] private float _duration = 0.5f;
        [Input(ShowBackingValue.Unconnected), SerializeField] private float _strength = 1;
        [Input(ShowBackingValue.Unconnected), SerializeField] private int _vibrato = 10;
        [Input(ShowBackingValue.Unconnected), SerializeField] private float _fadeOut = 0;

        public override async void Execute()
        {
            base.Execute();
            float inputDuration = GetInputValue(nameof(_duration), _duration);
            float inputStrength = GetInputValue(nameof(_strength), _strength);
            int inputVibrato = GetInputValue(nameof(_vibrato), _vibrato);
            float inputFadeOut = GetInputValue(nameof(_fadeOut), _fadeOut);

            var service = NovelGame.Instance.GetService<BackgroundService>();
            await service.ShakeScale(inputDuration, inputStrength, inputVibrato, inputFadeOut);
            StopTask();
        }
    }
}