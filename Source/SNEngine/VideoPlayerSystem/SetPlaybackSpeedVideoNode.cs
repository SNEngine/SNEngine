using UnityEngine;

namespace SNEngine.VideoPlayerSystem
{
    public class SetPlaybackSpeedVideoNode : VideoInteractionNode
    {
        [Input, SerializeField, Range(0, 10)] private float _playbackSpeed = 1;
        protected override void Interact(NovelVideoPlayer input)
        {
            var value = GetInputValue<float>(nameof(_playbackSpeed), _playbackSpeed);
            input.PlaybackSpeed = value;
        }
    }
}
