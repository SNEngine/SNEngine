using System;
using System.Collections.Generic;
using UnityEngine;
using SNEngine.Debugging;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SNEngine.Serialization
{
    public abstract class BaseAssetLibrary : ScriptableObjectIdentity
    {
        public abstract Type GetTypeAsset();
        public abstract object GetAsset(string guid);
        public abstract void Add(object asset);
    }

    public abstract partial class BaseAssetLibrary<T> : BaseAssetLibrary where T : UnityEngine.Object
    {
        [SerializeField] private List<Entry> _entries = new List<Entry>();

        public IReadOnlyList<Entry> Entries => _entries;

        public override void Add(object asset)
        {
            if (asset == null) return;

            var targetType = GetTypeAsset();
            if (asset.GetType() != targetType)
            {
                NovelGameDebug.LogError($"Invalid type asset for library {GetType().Name}. Expected: {targetType.Name}, Got: {asset.GetType().Name}");
                return;
            }

            T convertedAsset = asset as T;
            if (convertedAsset == null) return;

            string guid = string.Empty;

#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(convertedAsset);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            guid = AssetDatabase.AssetPathToGUID(path);
#endif

            if (string.IsNullOrEmpty(guid))
            {
                guid = Guid.NewGuid().ToString();
            }

            if (!_entries.Exists(e => e.Guid == guid))
            {
                _entries.Add(new Entry { Guid = guid, Asset = convertedAsset });
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }

        public override object GetAsset(string guid)
        {
            var entity = _entries.FirstOrDefault(x => x.Guid == guid);
            return entity?.Asset;
        }

        public string GetGuid(T asset)
        {
            var entity = _entries.FirstOrDefault(x => x.Asset == asset);
            return entity?.Guid;
        }

        public override Type GetTypeAsset()
        {
            return typeof(T);
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (_entries.RemoveAll(e => e.Asset == null) > 0)
            {
                EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}