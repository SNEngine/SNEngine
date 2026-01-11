using DG.Tweening;
using SNEngine.SpriteObjectSystem;
using UnityEngine;

namespace SNEngine.SpriteObjectSystem
{
    public class HideSpriteObjectNode : SpriteObjectInteractionNode
    {
        protected override void Interact(SpriteObject input)
        {
            input.Hide();
        }
    }

    public class MoveSpriteObjectNode : SpriteObjectInteractionNode
    {
        [Input(ShowBackingValue.Unconnected), SerializeField] private Vector3 _position = Vector3.zero;
        [Input(ShowBackingValue.Unconnected), SerializeField] private float _duration = 1;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Ease _ease = Ease.Linear;

        protected override async void Interact(SpriteObject input)
        {
            Vector3 inputPosition = GetInputValue(nameof(_position), _position);
            float inputDuration = GetInputValue(nameof(_duration), _duration);
            Ease inputEase = GetInputValue(nameof(_ease), _ease);

            await input.MoveTo(inputPosition, inputDuration, inputEase);
        }
    }
}