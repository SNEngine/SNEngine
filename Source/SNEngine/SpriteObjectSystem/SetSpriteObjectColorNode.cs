using DG.Tweening;
using UnityEngine;

namespace SNEngine.SpriteObjectSystem
{
    public class SetSpriteObjectColorNode : SpriteObjectInteractionNode
    {
        [Input(ShowBackingValue.Unconnected), SerializeField] private float _duration = 1;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Ease _ease = Ease.Linear;
        [Input(ShowBackingValue.Unconnected), SerializeField] private Color _color = Color.white;

        protected override async void Interact(SpriteObject input)
        {
            float inputDuration = GetInputValue(nameof(_duration), _duration);
            Ease inputEase = GetInputValue(nameof(_ease), _ease);
            Color inputColor = GetInputValue(nameof(_color), _color);

            await input.SetColor(inputColor, inputDuration, inputEase);
        }
    }
}