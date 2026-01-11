using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace SNEngine.Audio
{
    public interface IAudioObject : IResetable
    {
        bool Mute { get; set; }
        bool Loop { get; set; }
        float Volume { get; set; }
        float Pitch { get; set; }
        float PanStereo { get; set; }
        float SpatialBlend { get; set; }
        float ReverbZoneMix { get; set; }
        float DopplerLevel { get; set; }
        float Spread { get; set; }
        int Priority { get; set; }
        float MinDistance { get; set; }
        float MaxDistance { get; set; }
        AudioRolloffMode RolloffMode { get; set; }
        AudioClip CurrentSound { get; set; }
        AudioMixerGroup Mixer { get; set; }

        bool IsPlaying { get; }
        float TimePosition { get; set; }

        void Play();
        void PlayDelayed(float delay);
        void PlayScheduled(double time);
        void Stop();
        void Pause();
        void UnPause();
        void PlayOneShot(AudioClip clip, float volumeScale = 1f);

        void SetPosition(Vector3 position);
        void Set3DSettings(float spatialBlend = 1f, float minDistance = 1f, float maxDistance = 500f);

        UniTask FadeInAsync(float duration, float targetVolume = 1f);
        UniTask FadeOutAsync(float duration);
    }
}
