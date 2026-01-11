using UnityEngine;
using XNode;
namespace SNEngine.Audio.Music
{
    public class ClearPlaylistMusicNode : MusicInteractionNode
    {
        public override void Execute()
        {
            MusicPlayer.ClearPlaylist();
        }

        public override bool CanSkip()
        {
            return false;
        }
    }
}