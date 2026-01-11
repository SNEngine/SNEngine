using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Services;
using UnityEngine;

namespace SNEngine.BackgroundSystem.Animations
{
    public class SetBackgroundColourNode : AsyncNode
    {
        [Input(ShowBackingValue.Unconnected), SerializeField] private float _duration = 1;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Ease _ease = Ease.Linear;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Color _color = Color.white;

        public override async void Execute()
        {
            base.Execute();
            float inputDuration = GetInputValue(nameof(_duration), _duration);
            Ease inputEase = GetInputValue(nameof(_ease), _ease);
            Color inputColor = GetInputValue(nameof(_color), _color);

            var service = NovelGame.Instance.GetService<BackgroundService>();
            await service.SetColor(inputColor, inputDuration, inputEase);
            StopTask();
        }
    }
}