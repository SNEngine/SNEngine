using UnityEngine;

namespace SNEngine.VideoPlayerSystem
{
    public class SetLoopVideoNode : VideoInteractionNode
    {
        [Input, SerializeField] private bool _loop;
        protected override void Interact(NovelVideoPlayer input)
        {
            var value = GetInputValue<bool>(nameof(_loop), _loop);
            input.IsLooping = value;
        }
    }
}
