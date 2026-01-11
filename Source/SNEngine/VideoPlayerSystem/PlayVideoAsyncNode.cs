using Cysharp.Threading.Tasks;
using SNEngine.Attributes;
using SNEngine.Debugging;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace SNEngine.VideoPlayerSystem
{
    public class PlayVideoAsyncNode : VideoInteractionNodeAsync
    {
        [SerializeField, StreamingVideoPath(hideLabel: true)] private string _videoPath;

        protected override async UniTask Interact(NovelVideoPlayer input)
        {
            if (string.IsNullOrEmpty(_videoPath))
            {
                NovelGameDebug.LogError($"video path not seted for node {GUID}");
                StopTask();
                return;
            }

            input.URL = System.IO.Path.Combine(Application.streamingAssetsPath, _videoPath);
            input.Show();
            input.Play();

            await UniTask.WaitUntil(() => input.IsPlaying, cancellationToken: TokenSource.Token);
            await UniTask.WaitUntil(() => !input.IsPlaying, cancellationToken: TokenSource.Token);
            StopTask();
        }
    }
}