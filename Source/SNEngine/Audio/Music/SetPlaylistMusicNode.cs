using UnityEngine;
using XNode;
namespace SNEngine.Audio.Music
{
    public class SetPlaylistMusicNode : MusicInteractionNode
    {
        [XNode.Node.Input, SerializeField] private AudioClip[] _input;

        public override void Execute()
        {
            var input = GetInputValue(nameof(_input), _input);
            MusicPlayer.SetPlaylist(input);
        }

        public override bool CanSkip()
        {
            return false;
        }
    }
}
