using SiphoinUnityHelpers.XNodeExtensions;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SNEngine.Services;

namespace SNEngine.Audio.Music
{
    public abstract class MusicInteractionNodeAsync : AsyncNode
    {
        protected IMusicPlayer MusicPlayer
        {
            get
            {
                var service = NovelGame.Instance.GetService<MusicService>();
                return service.MusicPlayer;
            }
        }
    }
}
