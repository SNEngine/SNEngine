using Cysharp.Threading.Tasks;
using DG.Tweening;
using SNEngine.SpriteObjectSystem;
using UnityEngine;

namespace SNEngine.SpriteObjectSystem
{
    public class FadeSpriteObjectValueNode : SpriteObjectInteractionAsyncNode
    {
        [Input(ShowBackingValue.Unconnected), SerializeField, Range(0, 1)] private float _value = 0;
        [Input(ShowBackingValue.Unconnected), SerializeField] private float _duration = 1;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Ease _ease = Ease.Linear;

        protected override async UniTask Interact(SpriteObject input)
        {
            float inputValue = GetInputValue(nameof(_value), _value);
            float inputDuration = GetInputValue(nameof(_duration), _duration);
            Ease inputEase = GetInputValue(nameof(_ease), _ease);

            await input.Fade(inputValue, inputDuration, inputEase);
            
        }
    }
}