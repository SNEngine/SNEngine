using SNEngine.Audio;
using SNEngine.Audio.Models;
using SNEngine.Debugging;
using SNEngine.Polling;
using System;
using UnityEngine;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Audio Service")]
    public class AudioService : ServiceBase
    {
        private PoolMono<AudioObject> _audioObjects;
        [SerializeField, Min(1)] private int _sizePool = 9;
        public event Action<float> OnMusicVolumeChanged;
        public event Action<float> OnFXVolumeChanged;

        public event Action<bool> OnMusicMuteChanged;
        public event Action<bool> OnFXMuteChanged;

        public AudioData AudioData
        {
            get
            {
                var service = NovelGame.Instance.GetService<UserDataService>();
                if (service == null || service.Data == null)
                {
                    return new AudioData();
                }
                return service.Data.AudioData;
            }
        }

        public override void Initialize()
        {
            AudioObject _prefab = Resources.Load<AudioObject>("Audio/AudioObject");

            if (_prefab == null)
            {
                NovelGameDebug.LogError("AudioObject prefab not found in Resources/Audio/AudioObject");
                return;
            }

            Transform container = new GameObject($"{nameof(AudioObject)}_Container").transform;
            DontDestroyOnLoad(container.gameObject);
            _audioObjects = new PoolMono<AudioObject>(_prefab, container, _sizePool, true);
        }

        public IAudioObject PlaySound(AudioClip clip)
        {
            var newSound = GetFreeAudioObject();
            newSound.CurrentSound = clip;
            newSound.Play();
            return newSound;
        }

        public void StopSound(IAudioObject audioObject)
        {
            audioObject.Stop();
        }

        public void SetMute(IAudioObject audioObject, bool mute)
        {
            if (audioObject is null)
            {
                NovelGameDebug.LogError("audio object is null");
                return;
            }
            audioObject.Mute = mute;
        }

        public IAudioObject GetFreeAudioObject()
        {
            var element = _audioObjects.GetFreeElement();
            element.gameObject.SetActive(true);
            return element;
        }

        public void SetVolumeMusic(float volume)
        {
            AudioData.MusicVolumw = Mathf.Clamp01((float)volume);
            OnMusicVolumeChanged?.Invoke(volume);
        }

        public void SetVolumeFX(float volume)
        {
            AudioData.FXVolume = Mathf.Clamp01((float)volume);
            OnFXVolumeChanged?.Invoke(volume);
        }

        public void SetMuteMusic(float volume)
        {
            AudioData.MusicVolumw = Mathf.Clamp01((float)volume);
            OnMusicVolumeChanged?.Invoke(volume);
        }

        public void SetMuteFX(bool mute)
        {
            AudioData.MuteFX = mute;
            OnFXMuteChanged?.Invoke(mute);
        }

        public void SetMuteMusic(bool mute)
        {
            AudioData.MuteMusic = mute;
            OnMusicMuteChanged?.Invoke(mute);
        }

        public override void ResetState()
        {
            foreach (var item in _audioObjects.Objects)
            {
                item.ResetState();
            }
        }
    }
}