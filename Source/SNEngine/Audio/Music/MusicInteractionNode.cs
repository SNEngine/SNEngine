using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.Services;

namespace SNEngine.Audio.Music
{
    public abstract class MusicInteractionNode : BaseNodeInteraction
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
