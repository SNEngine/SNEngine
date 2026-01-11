using SNEngine.Debugging;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SNEngine.Utils
{
    public static class ResourceLoader
    {
        public static T LoadCustomOrVanilla<T>(string vanillaPath) where T : Object
        {
            string customPath = $"Custom/{vanillaPath}";

            T assetToLoad = Resources.Load<T>(customPath);

            if (assetToLoad != null)
            {
                return assetToLoad;
            }

            assetToLoad = Resources.Load<T>(vanillaPath);

            if (assetToLoad is null)
            {
                NovelGameDebug.LogError($"Failed to load resource of type {typeof(T).Name} from paths: {customPath} or {vanillaPath}");
            }

            return assetToLoad;
        }

        public static T[] LoadAllCustomizable<T>(string vanillaPath) where T : Object
        {
            string customPath = $"Custom/{vanillaPath}";
            var assets = new Dictionary<string, T>();

            var vanillaAssets = Resources.LoadAll<T>(vanillaPath);
            if (vanillaAssets != null)
            {
                foreach (var asset in vanillaAssets)
                {
                    if (asset != null) assets[asset.name] = asset;
                }
            }

            var customAssets = Resources.LoadAll<T>(customPath);
            if (customAssets != null)
            {
                foreach (var asset in customAssets)
                {
                    if (asset != null) assets[asset.name] = asset;
                }
            }

            if (assets.Count == 0)
            {
                NovelGameDebug.LogWarning($"No resources of type {typeof(T).Name} found in {vanillaPath} or {customPath}");
                return new T[0];
            }

            var result = new T[assets.Count];
            assets.Values.CopyTo(result, 0);

            return result;
        }
    }
}