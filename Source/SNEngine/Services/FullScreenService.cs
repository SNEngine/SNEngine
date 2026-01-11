using SNEngine.fullScreenSystem.Models;
using UnityEngine;
using System.Runtime.InteropServices;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Full Screen Service")]
    public class FullScreenService : ServiceBase
    {
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void SetUnityFullScreen(int enable);
#endif

        public FullScreenData Data => NovelGame.Instance.GetService<UserDataService>().Data.FullScreenData;

        public void SetFullScreen(bool isFullScreen)
        {
            Data.IsOn = isFullScreen;

#if UNITY_WEBGL
            int enable = isFullScreen ? 1 : 0;
            SetUnityFullScreen(enable);
#else
            Screen.fullScreen = isFullScreen;
#endif
        }
    }
}