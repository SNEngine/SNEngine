using Cysharp.Threading.Tasks;
using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using UnityEngine;
using XNode;

namespace SNEngine.BackgroundSystem.AsyncNodes
{
    public abstract class AsyncBackgroundNode : AsyncNode
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private float _duration = 1f;
        [Input(connectionType = ConnectionType.Override), SerializeField] private Ease _ease = Ease.Linear;

        public override void Execute()
        {
            base.Execute();

            float finalDuration = _duration;
            Ease finalEase = _ease;

            var durationPort = GetInputPort(nameof(_duration));
            if (durationPort != null && durationPort.Connection != null)
            {
                finalDuration = GetDataFromPort<float>(nameof(_duration));
            }

            var easePort = GetInputPort(nameof(_ease));
            if (easePort != null && easePort.Connection != null)
            {
                finalEase = GetDataFromPort<Ease>(nameof(_ease));
            }

            Play(finalDuration, finalEase);
        }

        protected abstract void Play(float duration, Ease ease);
    }
}