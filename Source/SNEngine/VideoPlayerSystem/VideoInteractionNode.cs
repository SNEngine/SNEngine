using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.Debugging;
using UnityEngine;

namespace SNEngine.VideoPlayerSystem
{
    public abstract class VideoInteractionNode : BaseNodeInteraction
    {
        [Input(ShowBackingValue.Never), SerializeField] private NovelVideoPlayer _input;

        public override void Execute()
        {
            var input = GetInputValue<NovelVideoPlayer>(nameof(_input));
            if (!input)
            {
                NovelGameDebug.LogError($"invalid video player input or input is null");
                return;
            }
            Interact(input);
        }

        protected abstract void Interact(NovelVideoPlayer input);
    }
}
