using SiphoinUnityHelpers.XNodeExtensions.Variables.DictionarySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables.DictionarySystem
{
    public abstract class DictionaryVariableNode : VariableNode, IDictionary
    {
        public abstract object this[object key] { get; set; }
        public abstract bool IsFixedSize { get; }
        public abstract bool IsReadOnly { get; }
        public abstract ICollection Keys { get; }
        public abstract ICollection Values { get; }
        public abstract int Count { get; }
        public abstract bool IsSynchronized { get; }
        public abstract object SyncRoot { get; }
        public abstract void Add(object key, object value);
        public abstract void Clear();
        public abstract bool Contains(object key);
        public abstract void CopyTo(Array array, int index);
        public abstract IDictionaryEnumerator GetEnumerator();
        public abstract void Remove(object key);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public abstract class DictionaryVariableNode<TKey, TValue> : DictionaryVariableNode, IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] protected List<DictionaryData<TKey, TValue>> _serializedItems = new List<DictionaryData<TKey, TValue>>();
        protected Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        protected Dictionary<TKey, TValue> _startValues = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set => _dictionary[key] = value;
        }

        public override object this[object key]
        {
            get => _dictionary[(TKey)key];
            set => _dictionary[(TKey)key] = (TValue)value;
        }

        public override ICollection Keys => ((IDictionary)_dictionary).Keys;
        public override ICollection Values => ((IDictionary)_dictionary).Values;
        public override int Count => _dictionary.Count;
        public override bool IsReadOnly => false;
        public override bool IsFixedSize => false;
        public override bool IsSynchronized => false;
        public override object SyncRoot => ((ICollection)_dictionary).SyncRoot;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => _dictionary.Keys;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => _dictionary.Values;

        public override void Add(object key, object value) => ((IDictionary)_dictionary).Add(key, value);
        public void Add(TKey key, TValue value) => _dictionary.Add(key, value);
        public void Add(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)_dictionary).Add(item);
        public override void Clear() => _dictionary.Clear();
        public bool Contains(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)_dictionary).Contains(item);
        public override bool Contains(object key) => ((IDictionary)_dictionary).Contains(key);
        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((IDictionary<TKey, TValue>)_dictionary).CopyTo(array, arrayIndex);
        public override void CopyTo(Array array, int index) => ((IDictionary)_dictionary).CopyTo(array, index);
        public override IDictionaryEnumerator GetEnumerator() => ((IDictionary)_dictionary).GetEnumerator();
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => _dictionary.GetEnumerator();
        public override void Remove(object key) => ((IDictionary)_dictionary).Remove(key);
        public bool Remove(TKey key) => _dictionary.Remove(key);
        public bool Remove(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)_dictionary).Remove(item);
        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        public override object GetCurrentValue() => _dictionary;
        public override object GetStartValue() => _startValues;
        public override void SetValue(object value) { if (value is Dictionary<TKey, TValue> d) _dictionary = d; }

        public override void ResetValue()
        {
            _dictionary = new Dictionary<TKey, TValue>(_startValues);
        }

        public void OnBeforeSerialize()
        {
            _serializedItems.Clear();
            foreach (var kvp in _dictionary)
            {
                _serializedItems.Add(new DictionaryData<TKey, TValue>(kvp.Key, kvp.Value));
            }
        }

        public void OnAfterDeserialize()
        {
            _dictionary.Clear();
            foreach (var item in _serializedItems)
            {
                if (item.Key != null && !_dictionary.ContainsKey(item.Key))
                    _dictionary.Add(item.Key, item.Value);
            }

            _startValues = new Dictionary<TKey, TValue>(_dictionary);
        }
    }
}