using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using SNEngine.Audio.Music;
using SNEngine.Debugging;
using SNEngine.Services;
using System;
using UnityEngine;

namespace SNEngine.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceController : MonoBehaviour
    {
        private AudioType? _type;
        private AudioService _service;
        [SerializeField, ReadOnly] private AudioSource _audioSource;
        [SerializeField] private AudioType _defaultAudioType;

        private float _settingsVolume = 1f;
        private float _multiplier = 1f;

        private void OnEnable()
        {
            if (!_service) _service = NovelGame.Instance.GetService<AudioService>();
            if (_service == null) return;

            if (_type is null)
            {
                if (_defaultAudioType == AudioType.None)
                {
                    _type = TryGetComponent(out MusicPlayer _) ? AudioType.Music : AudioType.FX;
                }
                else
                {
                    _type = _defaultAudioType;
                }
            }

            if (_service.AudioData == null) return;

            switch (_type.Value)
            {
                case AudioType.Music:
                    _service.OnMusicMuteChanged += OnMusicMuteChanged;
                    _service.OnMusicVolumeChanged += OnMusicVolumeChanged;
                    OnMusicMuteChanged(_service.AudioData.MuteMusic);
                    OnMusicVolumeChanged(_service.AudioData.MusicVolumw);
                    break;
                case AudioType.FX:
                    _service.OnFXMuteChanged += OnFXMuteChanged;
                    _service.OnFXVolumeChanged += OnFXVolumeChanged;
                    OnFXMuteChanged(_service.AudioData.MuteFX);
                    OnFXVolumeChanged(_service.AudioData.FXVolume);
                    break;
            }
        }

        // ЭТОТ МЕТОД НУЖНО ДОБАВИТЬ
        public void UpdateVolumeWithMultiplier(float multiplier)
        {
            _multiplier = multiplier;
            ApplyFinalVolume();
        }

        private void ApplyFinalVolume()
        {
            if (_audioSource)
            {
                _audioSource.volume = _settingsVolume * _multiplier;
            }
        }

        private void OnDisable()
        {
            if (_service == null || _type is null) return;

            switch (_type.Value)
            {
                case AudioType.Music:
                    _service.OnMusicMuteChanged -= OnMusicMuteChanged;
                    _service.OnMusicVolumeChanged -= OnMusicVolumeChanged;
                    break;
                case AudioType.FX:
                    _service.OnFXMuteChanged -= OnFXMuteChanged;
                    _service.OnFXVolumeChanged -= OnFXVolumeChanged;
                    break;
            }
        }

        private void OnMusicVolumeChanged(float volume)
        {
            _settingsVolume = volume;
            ApplyFinalVolume();
        }

        private void OnFXVolumeChanged(float value)
        {
            _settingsVolume = value;
            ApplyFinalVolume();
        }

        private void OnFXMuteChanged(bool mute) => _audioSource.mute = mute;
        private void OnMusicMuteChanged(bool mute) => _audioSource.mute = mute;

        private void OnValidate()
        {
            if (!_audioSource) _audioSource = GetComponent<AudioSource>();
        }
    }
}