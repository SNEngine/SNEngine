using System;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.DictionarySystem
{
    [Serializable]
    public class DictionaryData<TKey, TValue>
    {
        [SerializeField] private TKey _key;
        [SerializeField] private TValue _value;

        public TKey Key => _key;
        public TValue Value => _value;

        public DictionaryData(TKey key, TValue value)
        {
            _key = key;
            _value = value;
        }
    }
}