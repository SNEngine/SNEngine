using System.Linq;
using UnityEngine;
using SNEngine.Utils;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/New Service Container")]
    public class ServiceContainer : ScriptableObject
    {
        [SerializeField] private ServiceBase[] _services;

        private ServiceBase[] _allServices;

        private const string ServiceResourcePath = "Services";

        public void Initialize()
        {
            ServiceBase[] customServices = ResourceLoader.LoadAllCustomizable<ServiceBase>(ServiceResourcePath);

            _allServices = _services
                .Concat(customServices)
                .Distinct()
                .ToArray();

            foreach (var service in _allServices)
            {
                service.Initialize();
            }
        }

        internal T Get<T>() where T : ServiceBase
        {
            return _allServices?.FirstOrDefault(x => x is T) as T;
        }

        internal void ResetState()
        {
            if (_allServices is null) return;

            foreach (var item in _allServices)
            {
                item.ResetState();
            }
        }
    }
}