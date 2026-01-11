using Cysharp.Threading.Tasks;
using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Services;
using UnityEngine;
using XNode;

namespace SNEngine.BackgroundSystem.Animations
{
    public class SetBackgroundTransperentNode : AsyncNode
    {
        [Input(ShowBackingValue.Unconnected), SerializeField] private float _duration = 1;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Ease _ease = Ease.Linear;
        [Input(ShowBackingValue.Unconnected), Range(0, 1), SerializeField] private float _value = 1;

        public override async void Execute()
        {
            base.Execute();
            float inputDuration = GetInputValue(nameof(_duration), _duration);
            Ease inputEase = GetInputValue(nameof(_ease), _ease);
            float inputValue = GetInputValue(nameof(_value), _value);

            var service = NovelGame.Instance.GetService<BackgroundService>();
            await service.SetTransperent(inputValue, inputDuration, inputEase);
            StopTask();
        }
    }
}