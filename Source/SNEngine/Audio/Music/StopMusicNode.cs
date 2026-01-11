using UnityEngine;

namespace SNEngine.Audio.Music
{
    public class StopMusicNode : MusicInteractionNodeAsync
    {
        [XNode.Node.Input, SerializeField, Min(0)] private float _fadeDuration = 0;
        public override async void Execute()
        {
            var duration = GetInputValue(nameof(_fadeDuration), _fadeDuration);
            await MusicPlayer.StopAsync(duration);
            StopTask();
        }
    }

}
