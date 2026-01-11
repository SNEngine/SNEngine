using Cysharp.Threading.Tasks;
using DG.Tweening;
using SNEngine.BackgroundSystem.AsyncNodes;
using SNEngine.Services;
using UnityEngine;
using SNEngine.SaveSystem;

namespace SNEngine.BackgroundSystem.Animations.Celia
{
    public class CeliaBackgroundNode : AsyncBackgroundNode, ISaveProgressNode
    {
        private bool _isLoadFromSaveStub = false;

        [Input(connectionType = ConnectionType.Override), Range(0, 1), SerializeField] private float value;

        protected override void Play(float duration, Ease ease)
        {
            float finalValue = value;

            var input = GetInputPort(nameof(value));

            if (input != null && input.Connection != null)
            {
                finalValue = GetDataFromPort<float>(nameof(value));
            }

            float playDuration = _isLoadFromSaveStub ? 0f : duration;
            Ease playEase = _isLoadFromSaveStub ? Ease.Unset : ease;

            Celia(finalValue, playDuration, playEase).Forget();
        }

        private async UniTask Celia(float value, float duration, Ease ease)
        {
            var backgroundService = NovelGame.Instance.GetService<BackgroundService>();

            await backgroundService.Celia(duration, value, ease);

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