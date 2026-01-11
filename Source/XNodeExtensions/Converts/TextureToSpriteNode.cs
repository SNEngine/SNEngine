using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Converts
{
    [NodeTint("#4a6e82")]
    public class TextureToSpriteNode : BaseNodeInteraction
    {
        [Input(ShowBackingValue.Always), SerializeField] private Texture2D _texture;
        [Output, SerializeField] private Sprite _sprite;

        public override void Execute()
        {
            Texture2D tex = GetDataFromPort<Texture2D>(nameof(_texture));

            if (tex != null)
            {
                _sprite = Sprite.Create(
                    tex,
                    new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f),
                    100f
                );

                _sprite.name = tex.name;
            }
            else
            {
                _sprite = null;
            }

            if (Exit.Connection != null)
            {
                var nextNode = Exit.Connection.node as BaseNode;
                nextNode?.Execute();
            }
        }

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == nameof(_sprite)) return _sprite;
            return null;
        }
    }
}