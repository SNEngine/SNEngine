#if UNITY_WEBGL
using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using SNEngine.Audio.Music;
using SNEngine.Debugging;
using SNEngine.Services;
using UnityEngine;
using UnityEngine.Video;
using AudioType = SNEngine.Audio.AudioType;

namespace SNEngine.WebGL
{
    [RequireComponent(typeof(VideoPlayer))]
    public class WebGLVideoPlayerAudioSourceController : MonoBehaviour
    {
        private AudioType? _type;
        private AudioService _service;
        [SerializeField, ReadOnly] private VideoPlayer _videoPlayer;
        [SerializeField] private AudioType _defaultAudioType;

        private void OnEnable()
        {

            if (!_service)
            {
                _service = NovelGame.Instance.GetService<AudioService>();
            }
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
                default:
                    NovelGameDebug.LogError($"unkown type of audio type: {_type.Value}");
                    break;
            }
        }

        private void OnDisable()
        {
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
                default:
                    NovelGameDebug.LogError($"unkown type of audio type: {_type.Value}");
                    break;
            }
        }

        private void OnValidate()
        {
            if (!_videoPlayer)
            {
                _videoPlayer = GetComponent<VideoPlayer>();
            }
        }

        private void OnFXVolumeChanged(float value)
        {
            _videoPlayer.SetDirectAudioVolume(0, value);
        }

        private void OnMusicVolumeChanged(float volume)
        {
            _videoPlayer.SetDirectAudioVolume(0, volume);
        }

        private void OnFXMuteChanged(bool mute)
        {
            _videoPlayer.SetDirectAudioMute(0, mute);
        }

        private void OnMusicMuteChanged(bool mute)
        {
            _videoPlayer.SetDirectAudioMute(0, mute);
        }
    }
}
#endif