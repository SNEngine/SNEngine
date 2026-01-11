using System;

namespace SNEngine.Audio.Models
{
    [Serializable]
    public class AudioData
    {
        public float FXVolume { get; set; } = 0.5f;
        public float MusicVolumw { get; set; } = 0.5f;
        public bool MuteFX { get; set; }
        public bool MuteMusic { get; set; }
    }
}
