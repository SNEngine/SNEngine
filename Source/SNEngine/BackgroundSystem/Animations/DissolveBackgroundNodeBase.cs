using Cysharp.Threading.Tasks;
using DG.Tweening;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Animations;
using UnityEngine;
using XNode;

namespace SNEngine.BackgroundSystem.AsyncNodes
{
    public abstract class DissolveBackgroundNodeBase : AsyncNode
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private float _duration = 1f;
        [Input(connectionType = ConnectionType.Override), SerializeField] private AnimationBehaviourType _animationBehaviour = AnimationBehaviourType.In;
        [Input(connectionType = ConnectionType.Override), SerializeField] private Ease _ease = Ease.Linear;
        [Input(connectionType = ConnectionType.Override), SerializeField] private Texture2D _texture;

        public override void Execute()
        {
            base.Execute();

            float finalDuration = _duration;
            AnimationBehaviourType finalType = _animationBehaviour;
            Ease finalEase = _ease;
            Texture2D finalTexture = _texture;

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

            var texturePort = GetInputPort(nameof(_texture));
            if (texturePort != null && texturePort.Connection != null)
            {
                finalTexture = GetDataFromPort<Texture2D>(nameof(_texture));
            }

            Play(finalDuration, finalType, finalEase, finalTexture);
        }

        protected abstract void Play(float duration, AnimationBehaviourType type, Ease ease, Texture2D texture);

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}