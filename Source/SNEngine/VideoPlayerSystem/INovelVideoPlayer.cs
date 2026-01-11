using UnityEngine.Video;

namespace SNEngine.VideoPlayerSystem
{
    public interface INovelVideoPlayer : IShowable, IHidden, IResetable
    {
        VideoClip Clip { get; set; }
        string URL { get; set; }
        bool IsLooping { get; set; }
        double Time { get; set; }
        bool IsPlaying { get; }
        bool IsPrepared { get; }
        float PlaybackSpeed { get; set; }

        void PrepareAndPlay();
        void Play();
        void Pause();
        void Stop();
    }
}
