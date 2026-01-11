using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SNEngine.Debugging;
using SNEngine.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SNEngine.Serialisation
{
    public class UnityContractResolver : DefaultContractResolver
    {
        private readonly Dictionary<Type, JsonConverter> _assetConverters = new Dictionary<Type, JsonConverter>();

        private static readonly Dictionary<Type, string[]> ValueTypeMap = new Dictionary<Type, string[]>
        {
            { typeof(Vector2), new[] { "x", "y" } },
            { typeof(Vector3), new[] { "x", "y", "z" } },
            { typeof(Vector4), new[] { "x", "y", "z", "w" } },
            { typeof(Color), new[] { "r", "g", "b", "a" } },
            { typeof(Quaternion), new[] { "x", "y", "z", "w" } },
            { typeof(Rect), new[] { "x", "y", "width", "height" } }
        };

        public void RegisterLibrary(BaseAssetLibrary library)
        {
            Type assetType = library.GetTypeAsset();
            Type converterType = typeof(AssetConverter<>).MakeGenericType(assetType);

            var constructor = converterType.GetConstructors()[0];
            _assetConverters[assetType] = (JsonConverter)constructor.Invoke(new object[] { library });
        }
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            foreach (var kvp in _assetConverters)
            {
                if (kvp.Key.IsAssignableFrom(objectType)) return kvp.Value;
            }
            return base.ResolveContractConverter(objectType);
        }

        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            if (ValueTypeMap.TryGetValue(objectType, out var fields))
            {
                var members = new List<MemberInfo>();
                foreach (var name in fields)
                {
                    var f = objectType.GetField(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (f != null) members.Add(f);
                    var p = objectType.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (p != null) members.Add(p);
                }
                return members;
            }
            return base.GetSerializableMembers(objectType);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (ValueTypeMap.ContainsKey(member.DeclaringType))
            {
                property.Writable = true;
                property.Readable = true;
            }
            return property;
        }
    }
}