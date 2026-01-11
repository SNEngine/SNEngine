using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SNEngine.Audio.Music
{
    public interface IMusicPlayer : IResetable
    {
        float Volume { get; set; }
        bool Mute { get; set; }
        bool Loop { get; set; }
        bool IsPlaying { get; }
        AudioClip CurrentTrack { get; }

        void SetPlaylist(IEnumerable<AudioClip> playlist);
        void Play();
        void ClearPlaylist();
        void Pause();
        void UnPause();
        UniTask StopAsync(float fadeDuration = 1f);
        UniTask FadeVolumeAsync(float targetVolume, float duration);
    }
}
