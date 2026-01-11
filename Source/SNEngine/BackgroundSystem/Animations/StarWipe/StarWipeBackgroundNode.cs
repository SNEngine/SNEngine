using Cysharp.Threading.Tasks;
using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Animations;
using SNEngine.SaveSystem;
using SNEngine.Services;
using UnityEngine;
using XNode;

namespace SNEngine.BackgroundSystem.Animations.StarWipe
{
    public class StarWipeBackgroundNode : AsyncNode, ISaveProgressNode
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private float _duration = 1f;
        [Input(connectionType = ConnectionType.Override), SerializeField] private AnimationBehaviourType _type = AnimationBehaviourType.In;
        [Input(connectionType = ConnectionType.Override), SerializeField] private Ease _ease = Ease.Linear;

        private bool _isLoadFromSaveStub = false;

        public override void Execute()
        {
            base.Execute();

            float finalDuration = _duration;
            AnimationBehaviourType finalType = _type;
            Ease finalEase = _ease;

            var durationPort = GetInputPort(nameof(_duration));
            if (durationPort != null && durationPort.Connection != null)
            {
                finalDuration = GetDataFromPort<float>(nameof(_duration));
            }

            var typePort = GetInputPort(nameof(_type));
            if (typePort != null && typePort.Connection != null)
            {
                finalType = GetDataFromPort<AnimationBehaviourType>(nameof(_type));
            }

            var easePort = GetInputPort(nameof(_ease));
            if (easePort != null && easePort.Connection != null)
            {
                finalEase = GetDataFromPort<Ease>(nameof(_ease));
            }

            float playDuration = _isLoadFromSaveStub ? 0f : finalDuration;
            Ease playEase = _isLoadFromSaveStub ? Ease.Unset : finalEase;

            StarWipe(playDuration, finalType, playEase).Forget();
        }

        private async UniTask StarWipe(float duration, AnimationBehaviourType type, Ease ease)
        {
            var backgroundService = NovelGame.Instance.GetService<BackgroundService>();

            await backgroundService.StarWipe(duration, type, ease);

            StopTask();
        }

        public override object GetValue(NodePort port)
        {
            return null;
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