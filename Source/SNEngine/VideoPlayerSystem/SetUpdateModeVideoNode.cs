using System;
using UnityEngine;
using UnityEngine.Video;

namespace SNEngine.VideoPlayerSystem
{
    public class SetUpdateModeVideoNode : VideoInteractionNode
    {
        [Input, SerializeField, Range(0, 10)] private VideoTimeUpdateMode _updateMode =  VideoTimeUpdateMode.UnscaledGameTime;
        protected override void Interact(NovelVideoPlayer input)
        {
            var value = GetInputValue<VideoTimeUpdateMode>(nameof(_updateMode), _updateMode);
            input.UpdateMode = value;
        }
    }
}
