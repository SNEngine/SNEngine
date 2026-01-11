using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.Services;
using UnityEngine;
using XNode;

namespace SNEngine.VideoPlayerSystem
{
    public class GetVideoPlayerNode : BaseNodeInteraction
    {
        [Output(ShowBackingValue.Never), SerializeField] private NovelVideoPlayer _result;

        public override void Execute()
        {
            var service = NovelGame.Instance.GetService<VideoService>();
            _result = service.VideoPlayer as NovelVideoPlayer;
        }

        public override object GetValue(NodePort port)
        {
            return _result;
        }
    }
}
