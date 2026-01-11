using Newtonsoft.Json;
using SNEngine.Debugging;
using SNEngine.Serialisation;
using SNEngine.Serialization;
using SNEngine.Utils;
using System;
using System.Linq;
using UnityEngine;

namespace SNEngine.Services
{
    [CreateAssetMenu(menuName = "SNEngine/Services/Asset Library Service")]
    public class AssetLibraryService : ServiceBase
    {
        private UnityContractResolver _resolver;
        private BaseAssetLibrary[] _libraries;

        public override void Initialize()
        {
            _resolver = new();

            LoadLibraries();
            foreach (var library in _libraries)
            {
                _resolver.RegisterLibrary(library);
            }
            Formatting formatting = Formatting.Indented;

#if !UNITY_EDITOR
             formatting = Formatting.None;
#endif
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = _resolver,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = formatting
            };

            NovelGameDebug.Log($"JSON settings up");
        }

        private void LoadLibraries()
        {
            _libraries = ResourceLoader.LoadAllCustomizable<BaseAssetLibrary>("AssetLibraries");
        }

        public void AddAssetToLibrary<TLibrary> (UnityEngine.Object asset) where TLibrary : BaseAssetLibrary
        {
            if (_libraries is null || _libraries.Length == 0)
            {
                LoadLibraries();
            }
            Type type = asset.GetType();

            var library = _libraries.FirstOrDefault(x => x.GetTypeAsset() == type);
            if (!library)
            {
                NovelGameDebug.LogError($"asset library with Type asset {type.Name} not found");
            }

            library.Add(asset);
        }

        public TObject GetFromLibrary<TLibrary, TObject>(string guid) where TLibrary : BaseAssetLibrary where TObject : UnityEngine.Object
        {
            if (_libraries is null || _libraries.Length == 0)
            {
                LoadLibraries();
            }
            Type type = typeof(TObject);

            var library = _libraries.FirstOrDefault(x => x.GetTypeAsset() == type);
            if (!library)
            {
                NovelGameDebug.LogError($"asset library with Type asset {type.Name} not found");
            }

            return library.GetAsset(guid) as TObject;
        }
    }
}
