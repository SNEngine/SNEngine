using UnityEngine;
using XNode;
namespace SNEngine.Audio.Music
{
    public class PlayMusicNode : MusicInteractionNode
    {
        public override void Execute()
        {
            MusicPlayer.Play();
        }

        public override bool CanSkip()
        {
            return false;
        }
    }
}