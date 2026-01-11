using Cysharp.Threading.Tasks;
using SNEngine.Services;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace SNEngine
{
    public class MonoStratup : MonoBehaviour
    {
        private const string DEFAULT_LANGUAGE = "en";
        private async void Start()
        {
            await LoadLanguage();
            ShowMainMenu();
        }

        private async UniTask LoadLanguage()
        {
            var novelGame = NovelGame.Instance;
            var languageService = novelGame.GetService<LanguageService>();
            var userDataService = novelGame.GetService<UserDataService>();
            var token = this.GetCancellationTokenOnDestroy();
            await UniTask.WaitUntil(() => userDataService.DataIsLoaded, cancellationToken: token);
            var currentLanguage = userDataService.Data.CurrentLanguage;
            if (!string.IsNullOrEmpty(currentLanguage))
            {
                await languageService.LoadLanguage(currentLanguage);
            }
        }

        private void ShowMainMenu()
        {
            var novelGame = NovelGame.Instance;
            novelGame.GetService<MainMenuService>().Show();
        }
    }
}