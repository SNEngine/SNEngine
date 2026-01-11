using SNEngine.UserDataSystem.Models;
using UnityEngine;
using Cysharp.Threading.Tasks;
using SNEngine.Debugging;
using SNEngine.UserDataSystem;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/User Data Service")]
    public class UserDataService : ServiceBase, IService, IResetable
    {
        private UserData _data;
        private IUserDataProvider _provider;

        public UserData Data => _data;

        public bool DataIsLoaded => _data != null;

        public override async void Initialize()
        {
            base.Initialize();

#if UNITY_WEBGL
            _provider = new PlayerPrefsUserDataProvider();
            NovelGameDebug.Log("[UserDataService] Initialized with PlayerPrefsUserDataProvider for WebGL.");
#else
            _provider = new FileUserDataProvider();
            NovelGameDebug.Log("[UserDataService] Initialized with FileUserDataProvider for FileSystem.");
#endif

            _data = await LoadAsync();
        }

        public async UniTask<UserData> LoadAsync()
        {
            if (_provider == null)
            {
                NovelGameDebug.LogError("[UserDataService] Provider is not initialized.");
                return new UserData();
            }

            _data = await _provider.LoadAsync();

            _data ??= new UserData();
            return _data;
        }

        public UniTask SaveAsync()
        {
            if (_provider == null)
            {
                NovelGameDebug.LogError("[UserDataService] Provider is not initialized.");
                return UniTask.CompletedTask;
            }

            return _provider.SaveAsync(_data);
        }
    }
}