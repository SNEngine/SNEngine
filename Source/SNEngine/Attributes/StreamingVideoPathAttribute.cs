using UnityEngine;

namespace SNEngine.VideoPlayerSystem
{
    public class StreamingVideoPathAttribute : PropertyAttribute
    {
        public bool HideLabel { get; private set; }

        public StreamingVideoPathAttribute(bool hideLabel = false)
        {
            HideLabel = hideLabel;
        }
    }
}