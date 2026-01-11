using SNEngine.Services;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SNEngine.Audio.UI
{
    public class SettingsWindow : MonoBehaviour, ISettingsWindow
    {
        [SerializeField] private FillSlider _fxVolume;
        [SerializeField] private FillSlider _musicVolume;
        [SerializeField] private Toggle _toggleFX;
        [SerializeField] private Toggle _toggleMusic;
        [SerializeField] private Toggle _toggleFullScreen;

        private AudioService _audioService;

        private void OnFXChanged(float volume)
        {
            _audioService.SetVolumeFX(volume);
        }

        private void OnMusicChanged(float volume)
        {
            _audioService.SetVolumeMusic(volume);
        }

        private void OnToggleFX(bool fx)
        {
            _audioService.SetMuteFX(!fx);
        }

        private void OnToggleMusic(bool music)
        {
            _audioService.SetMuteMusic(!music);
        }

        private void OnEnable()
        {
            if (!_audioService)
            {
                _audioService = NovelGame.Instance.GetService<AudioService>();
            }

            _toggleMusic.isOn = !_audioService.AudioData.MuteMusic;
            _toggleFX.isOn = !_audioService.AudioData.MuteFX;
            _fxVolume.Value = _audioService.AudioData.FXVolume;
            _musicVolume.Value = _audioService.AudioData.MusicVolumw;
            _toggleFullScreen.isOn = NovelGame.Instance.GetService<FullScreenService>().Data.IsOn;

            

            _toggleMusic.onValueChanged.AddListener(OnToggleMusic);
            _toggleFX.onValueChanged.AddListener(OnToggleFX);
            _musicVolume.OnValueChanged.AddListener(OnMusicChanged);
            _fxVolume.OnValueChanged.AddListener(OnFXChanged);
#if UNITY_STANDALONE || UNITY_WEBGL
            _toggleFullScreen.onValueChanged.AddListener(FullScreenChanged);
#else
            Destroy(_toggleFullScreen.gameObject);
#endif
        }

        private void FullScreenChanged(bool fullScreen)
        {
            NovelGame.Instance.GetService<FullScreenService>().SetFullScreen(fullScreen);
        }

        private void OnDisable()
        {
            _toggleMusic.onValueChanged.RemoveListener(OnToggleMusic);
            _toggleFX.onValueChanged.RemoveListener(OnToggleFX);
            _musicVolume.OnValueChanged.RemoveListener(OnMusicChanged);
            _fxVolume.OnValueChanged.RemoveListener(OnFXChanged);
#if UNITY_STANDALONE || UNITY_WEBGL
            _toggleFullScreen.onValueChanged.RemoveListener(FullScreenChanged);
#endif

        }
    }
}