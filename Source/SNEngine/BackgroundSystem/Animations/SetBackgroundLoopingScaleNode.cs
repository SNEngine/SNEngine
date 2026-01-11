using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Services;
using UnityEngine;

namespace SNEngine.BackgroundSystem.Animations
{
    public class SetBackgroundLoopingScaleNode : AsyncNode
    {
        [Input(ShowBackingValue.Unconnected), SerializeField] private float _duration = 3;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Ease _ease = Ease.Linear;
        [Input(ShowBackingValue.Unconnected), SerializeField] private LoopType _loopType = LoopType.Yoyo;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Vector3 _endValue = new Vector3(1.1f, 1.1f, 1.1f);

        public override void Execute()
        {
            base.Execute();
            float inputDuration = GetInputValue(nameof(_duration), _duration);
            Ease inputEase = GetInputValue(nameof(_ease), _ease);
            LoopType inputLoopType = GetInputValue(nameof(_loopType), _loopType);
            Vector3 inputEndValue = GetInputValue(nameof(_endValue), _endValue);

            var service = NovelGame.Instance.GetService<BackgroundService>();
            service.SetLoopingScale(inputEndValue, inputDuration, inputLoopType, inputEase);
            StopTask();
        }
    }
}