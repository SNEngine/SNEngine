using SNEngine.Repositories;
using SNEngine.Services;
using System.Linq;
using UnityEngine;
using SNEngine.Utils;

namespace SNEngine.Repositories
{
    [CreateAssetMenu(menuName = "SNEngine/Repository Container")]
    public class RepositoryContainer : ScriptableObject
    {
        [SerializeField] private RepositoryBase[] _repositories;

        private RepositoryBase[] _allRepositories;

        private const string RepositoryResourcePath = "Repositories";

        public void Initialize()
        {
            RepositoryBase[] customRepositories = ResourceLoader.LoadAllCustomizable<RepositoryBase>(RepositoryResourcePath);

            _allRepositories = _repositories
                .Concat(customRepositories)
                .Distinct()
                .ToArray();

            foreach (var repository in _allRepositories)
            {
                repository.Initialize();
            }
        }

        internal T Get<T>() where T : RepositoryBase
        {
            return _allRepositories?.FirstOrDefault(x => x is T) as T;
        }
    }
}