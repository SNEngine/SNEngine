using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Services;
using UnityEngine;

namespace SNEngine.BackgroundSystem.Animations
{
    public class LookAtBackgroundNode : AsyncNode
    {
        [Input(ShowBackingValue.Unconnected), SerializeField] private float _duration = 1;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Ease _ease = Ease.Linear;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Vector3 _targetPosition = Vector3.zero;

        public override async void Execute()
        {
            base.Execute();
            float inputDuration = GetInputValue(nameof(_duration), _duration);
            Ease inputEase = GetInputValue(nameof(_ease), _ease);
            Vector3 inputTargetPosition = GetInputValue(nameof(_targetPosition), _targetPosition);

            var service = NovelGame.Instance.GetService<BackgroundService>();
            await service.LookAtTarget(inputTargetPosition, inputDuration, inputEase);
            StopTask();
        }
    }
}