using System;
using Newtonsoft.Json;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace SNEngine.Serialization
{
    public class AssetConverter<T> : JsonConverter<T> where T : UnityEngine.Object
    {
        private readonly BaseAssetLibrary<T> _library;
        public AssetConverter(BaseAssetLibrary<T> library) => _library = library;

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            if (value == null) { writer.WriteNull(); return; }
            string guid = _library.GetGuid(value);
            writer.WriteValue(guid);
        }

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            return _library.GetAsset((string)reader.Value) as T;
        }

    }

}