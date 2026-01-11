using Cysharp.Threading.Tasks;
using DG.Tweening;
using SNEngine.Animations;
using UnityEngine;
using static XNode.Node;

namespace SNEngine.SpriteObjectSystem
{
    public class FadeSpriteObjectBehaviourNode : SpriteObjectInteractionAsyncNode
    {
        [Input(ShowBackingValue.Unconnected), SerializeField] private AnimationBehaviourType _behaviour = AnimationBehaviourType.In;
        [Input(ShowBackingValue.Unconnected), SerializeField] private float _duration = 1;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Ease _ease = Ease.Linear;

        protected override async UniTask Interact(SpriteObject input)
        {
            AnimationBehaviourType inputBehaviour = GetInputValue(nameof(_behaviour), _behaviour);
            float inputDuration = GetInputValue(nameof(_duration), _duration);
            Ease inputEase = GetInputValue(nameof(_ease), _ease);

            await input.Fade(inputDuration, inputBehaviour, inputEase);
        }
    }
}