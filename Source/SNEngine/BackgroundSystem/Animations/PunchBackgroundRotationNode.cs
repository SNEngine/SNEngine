using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Services;
using UnityEngine;

namespace SNEngine.BackgroundSystem.Animations
{
    public class PunchBackgroundRotationNode : AsyncNode
    {
        [Input(ShowBackingValue.Unconnected), SerializeField] private float _duration = 0.5f;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Vector3 _punch = new Vector3(0, 0, 90);
        [Input(ShowBackingValue.Unconnected), SerializeField] private int _vibrato = 10;
        [Input(ShowBackingValue.Unconnected), SerializeField, Range(0, 1)] private float _elasticity = 1;

        public override async void Execute()
        {
            base.Execute();
            float inputDuration = GetInputValue(nameof(_duration), _duration);
            Vector3 inputPunch = GetInputValue(nameof(_punch), _punch);
            int inputVibrato = GetInputValue(nameof(_vibrato), _vibrato);
            float inputElasticity = GetInputValue(nameof(_elasticity), _elasticity);

            var service = NovelGame.Instance.GetService<BackgroundService>();
            await service.PunchRotation(inputPunch, inputDuration, inputVibrato, inputElasticity);
            StopTask();
        }
    }
}