using SNEngine.Serialization;
using SNEngine.Services;
using SNEngine.Utils;

namespace SNEngine.Serialisation
{
    public static class SNEngineSerialization
    {
        private static AssetLibraryService _assetLibraryService;
        public static void AddAssetToLibrary<TLibrary>(UnityEngine.Object asset) where TLibrary : BaseAssetLibrary
        {
            if (!_assetLibraryService)
            {
                _assetLibraryService = ResourceLoader.LoadCustomOrVanilla<AssetLibraryService>("Asset Library Service");
            }

            _assetLibraryService.AddAssetToLibrary<TLibrary>(asset);
        }

        public static TObject GetFromLibrary<TLibrary, TObject>(string guid) where TLibrary : BaseAssetLibrary where TObject : UnityEngine.Object
        {
            if (!_assetLibraryService)
            {
                _assetLibraryService = ResourceLoader.LoadCustomOrVanilla<AssetLibraryService>("Asset Library Service");
            }

            return _assetLibraryService.GetFromLibrary<TLibrary, TObject>(guid);
        }
    }
}
