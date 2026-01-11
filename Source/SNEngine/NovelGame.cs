using SNEngine.Debugging;
using SNEngine.Repositories;
using SNEngine.SaveSystem;
using SNEngine.Services;
using UnityEngine;

namespace SNEngine
{
    [RequireComponent(typeof(MonoStratup))]
    [RequireComponent(typeof(UserDataSaver))]
    public class NovelGame : MonoBehaviour, INovelGame
    {
        [SerializeField] private ServiceContainer _serviceContainer;
        [SerializeField] private RepositoryContainer _repositoryContainer;



        public static INovelGame Instance { get; private set; }
        private void Awake()
        {
            if (Instance is null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            else
            {
                Destroy(gameObject);
            }
            _repositoryContainer.Initialize();
            _serviceContainer.Initialize();
        }

        public T GetService<T>() where T : ServiceBase
        {
            return _serviceContainer.Get<T>();
        }

        public T GetRepository<T>() where T : RepositoryBase
        {
            return _repositoryContainer.Get<T>();
        }

        public void ResetStateServices ()
        {
            NovelGameDebug.Log("Clear Screen");

            _serviceContainer.ResetState();
        }
    }
}
