using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Services;
using UnityEngine;

namespace SNEngine.BackgroundSystem.Animations
{
    public class RotateBackgroundNode : AsyncNode
    {
        [Input(ShowBackingValue.Unconnected), SerializeField] private float _duration = 1;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Ease _ease = Ease.Linear;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Vector3 _rotation = Vector3.zero;

        public override async void Execute()
        {
            base.Execute();
            float inputDuration = GetInputValue(nameof(_duration), _duration);
            Ease inputEase = GetInputValue(nameof(_ease), _ease);
            Vector3 inputRotation = GetInputValue(nameof(_rotation), _rotation);

            var service = NovelGame.Instance.GetService<BackgroundService>();
            await service.RotateTo(inputRotation, inputDuration, inputEase);
            StopTask();
        }
    }
}