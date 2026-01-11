using Cysharp.Threading.Tasks;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Debugging;
using UnityEngine;

namespace SNEngine.VideoPlayerSystem
{
    public abstract class VideoInteractionNodeAsync : AsyncNode
    {
        [Input(ShowBackingValue.Never), SerializeField] private NovelVideoPlayer _input;

        public override void Execute()
        {
            base.Execute();
            var input = GetInputValue<NovelVideoPlayer>(nameof(_input));
            if (!input)
            {
                NovelGameDebug.LogError($"invalid video player input or input is null");
                return;
            }
            Interact(input).Forget();
        }

        protected abstract UniTask Interact(NovelVideoPlayer input);
    }
}
