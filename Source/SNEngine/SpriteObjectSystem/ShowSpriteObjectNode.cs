using UnityEngine;
using XNode;

namespace SNEngine.SpriteObjectSystem
{
    public class ShowSpriteObjectNode : SpriteObjectInteractionNode
    {
        [Input(ShowBackingValue.Unconnected), SerializeField] private Sprite _sprite;

        protected override async void Interact(SpriteObject input)
        {
            Sprite inputSprite = GetInputValue(nameof(_sprite), _sprite);

            if (inputSprite != null)
            {
                input.SetSprite(inputSprite);
            }

            input.Show();
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == nameof(_sprite)) return _sprite;
            return null;
        }
    }
}