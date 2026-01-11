using Cysharp.Threading.Tasks;
using SNEngine.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace SNEngine.Audio.Music
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(AudioSourceController))]
    public class MusicPlayer : MonoBehaviour, IMusicPlayer
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioSourceController _controller; // Добавили ссылку
        private CancellationTokenSource _fadeCts;
        private Queue<AudioClip> _currentQueue;
        private List<AudioClip> _originalPlaylist;

        private float _internalVolume = 1f;
        public float Volume
        {
            get => _internalVolume;
            set
            {
                _internalVolume = Mathf.Clamp01(value);
                UpdateFinalVolume();
            }
        }

        public bool IsPlaying => _audioSource.isPlaying;
        public AudioClip CurrentTrack => _audioSource.clip;

        public bool Mute
        {
            get => _audioSource.mute;
            set => _audioSource.mute = value;
        }

        private bool _loop;
        public bool Loop
        {
            get => _loop;
            set
            {
                if (_originalPlaylist?.Count > 1 && value)
                {
                    NovelGameDebug.LogError("Looping works only for a single track playlist.");
                    _loop = false;
                    return;
                }
                _loop = value;
                _audioSource.loop = value;
            }
        }

        private void Awake()
        {
            _currentQueue = new Queue<AudioClip>();
            if (!_controller) _controller = GetComponent<AudioSourceController>();
        }

        private void UpdateFinalVolume()
        {
            if (_controller != null)
            {
                _controller.UpdateVolumeWithMultiplier(_internalVolume);
            }
            else
            {
                _audioSource.volume = _internalVolume;
            }
        }

        public void SetPlaylist(IEnumerable<AudioClip> playlist)
        {
            if (playlist is null) return;

            _originalPlaylist = new List<AudioClip>(playlist);
            _currentQueue = new Queue<AudioClip>(_originalPlaylist);
        }

        public void Play()
        {
            if (_originalPlaylist != null && _originalPlaylist.Count > 0)
            {
                _currentQueue = new Queue<AudioClip>(_originalPlaylist);
                PlayNextTrackAsync().Forget();
            }
        }

        public void ClearPlaylist()
        {
            _originalPlaylist = null;
            _currentQueue.Clear();
            _audioSource.Stop();
        }

        public void Pause() => _audioSource.Pause();
        public void UnPause() => _audioSource.UnPause();

        public async UniTask StopAsync(float fadeDuration = 1f)
        {
            CancelFade();
            _fadeCts = new CancellationTokenSource();
            await FadeRoutineAsync(_internalVolume, 0f, fadeDuration, _fadeCts.Token, stopAfterFade: true);
        }

        private async UniTaskVoid PlayNextTrackAsync()
        {
            if (_currentQueue.Count == 0)
            {
                _audioSource.Stop();
                return;
            }

            var clip = _currentQueue.Dequeue();
            _audioSource.clip = clip;

            if (_internalVolume <= 0.001f)
            {
                Volume = 1f;
            }

            _audioSource.Play();

            try
            {
                while (_audioSource.isPlaying)
                    await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());

                if (_loop && _currentQueue.Count == 0 && _originalPlaylist?.Count == 1)
                {
                    // For single track looping, re-add the same clip
                    _currentQueue.Enqueue(_originalPlaylist[0]);
                    PlayNextTrackAsync().Forget();
                }
                else if (_currentQueue.Count > 0)
                {
                    // Continue with next track in queue
                    PlayNextTrackAsync().Forget();
                }
                // If no more tracks and not looping, just stop
            }
            catch (OperationCanceledException) { }
        }

        public async UniTask FadeVolumeAsync(float targetVolume, float duration)
        {
            CancelFade();
            _fadeCts = new CancellationTokenSource();
            await FadeRoutineAsync(_internalVolume, targetVolume, duration, _fadeCts.Token);
        }

        private async UniTask FadeRoutineAsync(float from, float to, float duration, CancellationToken token, bool stopAfterFade = false)
        {
            if (duration <= 0)
            {
                Volume = to;
                if (stopAfterFade) _audioSource.Stop();
                return;
            }

            float elapsed = 0f;
            if (!_audioSource.isPlaying && to > 0f) _audioSource.Play();

            try
            {
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    Volume = Mathf.Lerp(from, to, elapsed / duration);
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }

                Volume = to;
                if (stopAfterFade) _audioSource.Stop();
            }
            catch (OperationCanceledException) { }
        }

        private void CancelFade()
        {
            if (_fadeCts != null)
            {
                _fadeCts.Cancel();
                _fadeCts.Dispose();
                _fadeCts = null;
            }
        }

        private void OnDestroy() => CancelFade();

        public void ResetState()
        {
            CancelFade();
            _audioSource.Stop();
            Volume = 1f;
            Mute = false;
            Loop = false;
            _currentQueue.Clear();
            _originalPlaylist = null;
        }
    }
}