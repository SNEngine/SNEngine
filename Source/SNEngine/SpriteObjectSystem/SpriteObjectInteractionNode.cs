using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.Debugging;
using SNEngine.SpriteObjectSystem;
using UnityEngine;

namespace SNEngine.SpriteObjectSystem
{
    public abstract class SpriteObjectInteractionNode : BaseNodeInteraction
    {
        [Input(ShowBackingValue.Never), SerializeField] private SpriteObject _input;

        public override void Execute()
        {
            var inputObject = GetInputValue<SpriteObject>(nameof(_input));
            if (inputObject == null)
            {
                NovelGameDebug.LogError($"invalid sprite object input or input is null");
                return;
            }
            Interact(inputObject);
        }

        protected abstract void Interact(SpriteObject input);
    }
}