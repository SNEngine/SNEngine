using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;

namespace SNEngine.Audio
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(AudioSourceController))]
    public class AudioObject : MonoBehaviour, IAudioObject
    {
        [SerializeField] private AudioSource _audioSource;
        private CancellationTokenSource _cts;
        private AudioMixerGroup _defaultMixer;
        private static readonly Dictionary<int, int> _runtimeAudioRefCount = new();

        public bool Mute { get => _audioSource.mute; set => _audioSource.mute = value; }
        public bool Loop { get => _audioSource.loop; set => _audioSource.loop = value; }

        public float Volume { get => _audioSource.volume; set => _audioSource.volume = Mathf.Clamp01(value); }
        public float Pitch { get => _audioSource.pitch; set => _audioSource.pitch = Mathf.Clamp(value, -3f, 3f); }
        public float PanStereo { get => _audioSource.panStereo; set => _audioSource.panStereo = Mathf.Clamp(value, -1f, 1f); }
        public float SpatialBlend { get => _audioSource.spatialBlend; set => _audioSource.spatialBlend = Mathf.Clamp01(value); }

        public float ReverbZoneMix { get => _audioSource.reverbZoneMix; set => _audioSource.reverbZoneMix = Mathf.Clamp(value, 0f, 1.1f); }
        public float DopplerLevel { get => _audioSource.dopplerLevel; set => _audioSource.dopplerLevel = Mathf.Max(0f, value); }
        public float Spread { get => _audioSource.spread; set => _audioSource.spread = Mathf.Clamp(value, 0f, 360f); }
        public int Priority { get => _audioSource.priority; set => _audioSource.priority = (int)Mathf.Clamp(value, 0, 256); }

        public float MinDistance { get => _audioSource.minDistance; set => _audioSource.minDistance = Mathf.Max(0f, value); }
        public float MaxDistance { get => _audioSource.maxDistance; set => _audioSource.maxDistance = Mathf.Max(value, _audioSource.minDistance + 0.1f); }

        public AudioRolloffMode RolloffMode { get => _audioSource.rolloffMode; set => _audioSource.rolloffMode = value; }

        public AudioClip CurrentSound
        {
            get => _audioSource.clip;
            set
            {
                if (_audioSource.clip != null) TryCleanupRuntimeAudio();
                _audioSource.clip = value;
                if (value != null) RegisterRuntimeAudio(value);
            }
        }
        public AudioMixerGroup Mixer { get => _audioSource.outputAudioMixerGroup; set => _audioSource.outputAudioMixerGroup = value; }

        public bool IsPlaying => _audioSource.isPlaying;
        public float TimePosition { get => _audioSource.time; set => _audioSource.time = Mathf.Clamp(value, 0f, _audioSource.clip != null ? _audioSource.clip.length : 0f); }

        private void Awake()
        {
            _defaultMixer = Mixer;
        }

        public void Play()
        {
            _audioSource?.Play();
            WatchPlaybackAsync(_cts?.Token ?? this.GetCancellationTokenOnDestroy()).Forget();
        }

        public void PlayDelayed(float delay)
        {
            _audioSource?.PlayDelayed(delay);
            WatchPlaybackAsync(_cts?.Token ?? this.GetCancellationTokenOnDestroy()).Forget();
        }

        public void PlayScheduled(double time)
        {
            _audioSource?.PlayScheduled(time);
            WatchPlaybackAsync(_cts?.Token ?? this.GetCancellationTokenOnDestroy()).Forget();
        }
        public void Stop() => _audioSource?.Stop();
        public void Pause() => _audioSource?.Pause();
        public void UnPause() => _audioSource?.UnPause();

        public void PlayOneShot(AudioClip clip, float volumeScale = 1f)
        {
            CurrentSound = clip;
            Volume = volumeScale;
            Loop = false;
            Play();
        }

        public void SetPosition(Vector3 position) => _audioSource.transform.position = position;


        private async UniTaskVoid WatchPlaybackAsync(CancellationToken token)
        {
            try
            {
                while (_audioSource != null && _audioSource.isPlaying)
                    await UniTask.Yield(PlayerLoopTiming.Update, token);

                if (_audioSource != null && !_audioSource.loop && _audioSource.clip != null)
                    ResetState();
            }
            catch (OperationCanceledException)
            {
            }
        }

        public void Set3DSettings(float spatialBlend = 1f, float minDistance = 1f, float maxDistance = 500f)
        {
            SpatialBlend = spatialBlend;
            MinDistance = minDistance;
            MaxDistance = maxDistance;
        }

        private bool IsRuntimeAudio(AudioClip clip)
        {
            if (clip == null) return false;
            return clip.name.StartsWith("download_audio_") || clip.name.StartsWith("[Remote]");
        }

        private void RegisterRuntimeAudio(AudioClip clip)
        {
            if (!IsRuntimeAudio(clip)) return;

            int id = clip.GetInstanceID();
            if (_runtimeAudioRefCount.ContainsKey(id))
                _runtimeAudioRefCount[id]++;
            else
                _runtimeAudioRefCount[id] = 1;
        }

        private void TryCleanupRuntimeAudio()
        {
            AudioClip clip = _audioSource.clip;
            if (clip == null || !IsRuntimeAudio(clip)) return;

            int id = clip.GetInstanceID();
            if (_runtimeAudioRefCount.ContainsKey(id))
            {
                _runtimeAudioRefCount[id]--;
                if (_runtimeAudioRefCount[id] <= 0)
                {
                    _runtimeAudioRefCount.Remove(id);
                    Destroy(clip);
                }
            }
        }

        public async UniTask FadeInAsync(float duration, float targetVolume = 1f)
        {
            CancelFade();
            _cts = new CancellationTokenSource();

            float startVolume = _audioSource.volume;
            if (!_audioSource.isPlaying)
                _audioSource.Play();

            await FadeRoutineAsync(startVolume, targetVolume, duration, _cts.Token);
        }

        public async UniTask FadeOutAsync(float duration)
        {
            CancelFade();
            _cts = new CancellationTokenSource();

            float startVolume = _audioSource.volume;
            await FadeRoutineAsync(startVolume, 0f, duration, _cts.Token, stopAfterFade: true);
        }

        private async UniTask FadeRoutineAsync(float from, float to, float duration, CancellationToken token, bool stopAfterFade = false)
        {
            float elapsed = 0f;

            try
            {
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    _audioSource.volume = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }

                _audioSource.volume = to;

                if (stopAfterFade)
                    _audioSource.Stop();
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void CancelFade()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
        }

        private void OnDestroy()
        {
            CancelFade();
            if (_audioSource.clip != null) TryCleanupRuntimeAudio();
        }

        public void ResetState()
        {
            TryCleanupRuntimeAudio();
            Stop();
            MinDistance = 1;
            MaxDistance = 500;
            CurrentSound = null;
            Pitch = 1;
            Volume = 1;
            Priority = 128;
            Mute = false;
            Loop = false;
            Mixer = _defaultMixer;
            gameObject.SetActive(false);
            
        }
    }
}
