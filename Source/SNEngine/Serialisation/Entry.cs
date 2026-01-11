using System;
using UnityEngine;


#if UNITY_EDITOR
#endif

namespace SNEngine.Serialization
{
    public abstract partial class BaseAssetLibrary<T> where T : UnityEngine.Object
    {
        [Serializable]
        public class Entry
        {
            [field: SerializeField] public string Guid { get; set; }
            [field: SerializeField] public T Asset { get; set; }
        }
    }

}