using Cysharp.Threading.Tasks;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Debugging;
using UnityEngine;

namespace SNEngine.SpriteObjectSystem
{
    public abstract class SpriteObjectInteractionAsyncNode : AsyncNode
    {
        [Input(ShowBackingValue.Never), SerializeField] private SpriteObject _input;

        public override async void Execute()
        {
            base.Execute();
            var inputObject = GetInputValue<SpriteObject>(nameof(_input));
            if (inputObject == null)
            {
                NovelGameDebug.LogError($"invalid sprite object input or input is null");
                return;
            }
            await Interact(inputObject);
            StopTask();
        }

        protected abstract UniTask Interact(SpriteObject input);
    }
}