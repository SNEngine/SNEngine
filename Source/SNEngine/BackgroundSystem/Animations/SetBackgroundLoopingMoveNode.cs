using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.SaveSystem;
using SNEngine.Services;
using UnityEngine;

namespace SNEngine.BackgroundSystem.Animations
{

    public class SetBackgroundLoopingMoveNode : AsyncNode, ISaveProgressNode
    {
        private bool _isLoadFromSaveStub = false;

        [Input(ShowBackingValue.Unconnected), SerializeField] private float _duration = 3;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Ease _ease = Ease.Linear;
        [Input(ShowBackingValue.Unconnected), SerializeField] private LoopType _loopType = LoopType.Yoyo;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Vector3 _endValue = new Vector3(0.5f, 0, 0);

        public override void Execute()
        {
            base.Execute();
            float inputDuration = GetInputValue(nameof(_duration), _duration);
            Ease inputEase = GetInputValue(nameof(_ease), _ease);
            LoopType inputLoopType = GetInputValue(nameof(_loopType), _loopType);
            Vector3 inputEndValue = GetInputValue(nameof(_endValue), _endValue);

            var service = NovelGame.Instance.GetService<BackgroundService>();

            service.SetLoopingMove(inputEndValue, inputDuration, inputLoopType, inputEase);

            StopTask();
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