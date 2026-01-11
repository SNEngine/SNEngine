using SNEngine.Attributes;
using SNEngine.Debugging;
using UnityEngine;
using UnityEngine.Video;

namespace SNEngine.VideoPlayerSystem
{
    public class PlayVideoNode : VideoInteractionNode
    {
        [SerializeField, StreamingVideoPath] private string _videoPath;

        protected override void Interact(NovelVideoPlayer input)
        {
            if (string.IsNullOrEmpty(_videoPath))
            {
                NovelGameDebug.LogError($"video path not seted for node {GUID}");
                return;
            }

            input.URL = System.IO.Path.Combine(Application.streamingAssetsPath, _videoPath);
            input.Show();
            input.Play();
        }
    }
}