using Cysharp.Threading.Tasks;
using SNEngine.Services;
using UnityEngine;

namespace SNEngine.SaveSystem
{
    public class UserDataSaver : MonoBehaviour
    {
        private UserDataService _userDataService;

        private void Awake()
        {
            _userDataService = NovelGame.Instance.GetService<UserDataService>();
        }

        private void OnApplicationQuit()
        {
            _userDataService = NovelGame.Instance.GetService<UserDataService>();
            _userDataService.SaveAsync().Forget();
            PlayerPrefs.Save();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                _userDataService = NovelGame.Instance.GetService<UserDataService>();
                _userDataService.SaveAsync().Forget();
                PlayerPrefs.Save();
            }
        }
    }
}