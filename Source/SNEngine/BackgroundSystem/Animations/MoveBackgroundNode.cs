using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Services;
using SNEngine.SaveSystem;
using UnityEngine;

namespace SNEngine.BackgroundSystem.Animations
{
    public class MoveBackgroundNode : AsyncNode, ISaveProgressNode
    {
        private bool _isLoadFromSaveStub = false;

        [Input(ShowBackingValue.Unconnected), SerializeField] private float _duration = 1;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Ease _ease = Ease.Linear;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Vector3 _position = Vector3.zero;

        public override async void Execute()
        {
            base.Execute();
            float inputDuration = GetInputValue(nameof(_duration), _duration);
            Ease inputEase = GetInputValue(nameof(_ease), _ease);
            Vector3 inputPosition = GetInputValue(nameof(_position), _position);

            float moveDuration = _isLoadFromSaveStub ? 0f : inputDuration;
            Ease moveEase = _isLoadFromSaveStub ? Ease.Unset : inputEase;

            var service = NovelGame.Instance.GetService<BackgroundService>();
            await service.MoveTo(inputPosition, moveDuration, moveEase);
            StopTask();
        }
        public override bool CanSkip()
        {
            return false;
        }
        public object GetDataForSave()
        {
            return null;
        }

        public void SetDataFromSave(object data)
        {
            _isLoadFromSaveStub = true;
        }

        public void ResetSaveBehaviour()
        {
            _isLoadFromSaveStub = false;
        }
    }
}