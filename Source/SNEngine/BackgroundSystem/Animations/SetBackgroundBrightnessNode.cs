using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Services;
using UnityEngine;

namespace SNEngine.BackgroundSystem.Animations
{
    public class SetBackgroundBrightnessNode : AsyncNode
    {
        [Input(ShowBackingValue.Unconnected), SerializeField] private float _duration = 1;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Ease _ease = Ease.Linear;
        [Input(ShowBackingValue.Unconnected), Range(0, 1), SerializeField] private float _brightnessValue = 1;

        public override async void Execute()
        {
            base.Execute();
            float inputDuration = GetInputValue(nameof(_duration), _duration);
            Ease inputEase = GetInputValue(nameof(_ease), _ease);
            float inputValue = GetInputValue(nameof(_brightnessValue), _brightnessValue);

            var service = NovelGame.Instance.GetService<BackgroundService>();
            await service.SetBrightness(inputValue, inputDuration, inputEase);
            StopTask();
        }
    }
}