using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Animations;
using UnityEngine;

namespace SNEngine.BackgroundSystem.AsyncNodes
{
    public abstract class AsyncBackgroundInOutNode : AsyncNode
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private float _duration = 1f;
        [Input(connectionType = ConnectionType.Override), SerializeField] private AnimationBehaviourType _animationBehaviour = AnimationBehaviourType.In;
        [Input(connectionType = ConnectionType.Override), SerializeField] private Ease _ease = Ease.Linear;

        public override void Execute()
        {
            base.Execute();

            float finalDuration = _duration;
            AnimationBehaviourType finalType = _animationBehaviour;
            Ease finalEase = _ease;

            var durationPort = GetInputPort(nameof(_duration));
            if (durationPort != null && durationPort.Connection != null)
            {
                finalDuration = GetDataFromPort<float>(nameof(_duration));
            }

            var typePort = GetInputPort(nameof(_animationBehaviour));
            if (typePort != null && typePort.Connection != null)
            {
                finalType = GetDataFromPort<AnimationBehaviourType>(nameof(_animationBehaviour));
            }

            var easePort = GetInputPort(nameof(_ease));
            if (easePort != null && easePort.Connection != null)
            {
                finalEase = GetDataFromPort<Ease>(nameof(_ease));
            }

            Play(finalDuration, finalType, finalEase);
        }

        protected abstract void Play(float duration, AnimationBehaviourType type, Ease ease);
    }
}