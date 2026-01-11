using SNEngine.Audio.Music;
using SNEngine.Debugging;
using SNEngine.Services;
using UnityEngine;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Music Service")]
    public class MusicService : ServiceBase
    {
        public IMusicPlayer MusicPlayer { get; private set; }

        public override void Initialize()
        {
            var prefab = Resources.Load<MusicPlayer>("Audio/MusicPlayer");
            if (prefab == null)
            {
                NovelGameDebug.LogError("MusicPlayer prefab not found in Resources/Audio.");
                return;
            }

            var instance = Object.Instantiate(prefab);
            Object.DontDestroyOnLoad(instance);
            MusicPlayer = instance;
        }

        public override void ResetState()
        {
          MusicPlayer.ResetState();
        }
    }
}
