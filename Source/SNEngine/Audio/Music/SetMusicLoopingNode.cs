using System;
using UnityEngine;

namespace SNEngine.Audio.Music
{
    public class SetMusicLoopingNode : MusicInteractionNode
    {
        [XNode.Node.Input, SerializeField] private bool _flag = false;
        public override void Execute()
        {
            var flag = GetInputValue(nameof(_flag), _flag);
            MusicPlayer.Loop = flag;
        }
    }
}
