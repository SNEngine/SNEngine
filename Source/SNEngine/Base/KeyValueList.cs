using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace SNEngine
{
    [Serializable]
    public class KeyValueList<TKey, TValue> : IEnumerable<KeyValueElement<TKey, TValue>>
    {
        [SerializeField] private List<KeyValueElement<TKey, TValue>> _elements = new();

        private IEnumerable<KeyValueElement<TKey, TValue>> Elements => _elements;

        public int Count => _elements.Count;

        public KeyValueList()
        {

        }

        public KeyValueList(KeyValueList<TKey, TValue> elements)
        {
            _elements = new();

            foreach (var item in elements.Elements)
            {
                KeyValueElement<TKey, TValue> newElement = new(item.Key, item.Value);
                _elements.Add(newElement);
            }
        }

        public KeyValueList(Dictionary<TKey, TValue> elements)
        {
            _elements = new();

            foreach (var item in elements)
            {
                KeyValueElement<TKey, TValue> newElement = new(item.Key, item.Value);
                _elements.Add(newElement);
            }
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                TKey[] keys = new TKey[Count];
                for (int i = 0; i < Count; i++)
                {
                    keys[i] = _elements[i].Key;
                }

                return keys;
            }
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                TValue[] values = new TValue[Count];
                for (int i = 0; i < Count; i++)
                {
                    values[i] = _elements[i].Value;
                }

                return values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                var element = _elements.SingleOrDefault(x => x.Key.Equals(key));

                if (element is null)
                {
                    throw new KeyNotFoundException($"element in list by key {key} not found");
                }

                return element.Value;
            }
        }

        public TValue this[int index]
        {
            get
            {
                var element = _elements.ElementAt(index);
                if (element is null)
                {
                    throw new KeyNotFoundException($"element in list by index {index} not found");
                }

                return element.Value;
            }
        }

        public bool Contains(TValue value)
        {
            return _elements.Any(x => x.Value.Equals(value));
        }

        public bool Contains(TKey key)
        {
            return _elements.Any(x => x.Key.Equals(key));
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var element = _elements.FirstOrDefault(x => x.Key.Equals(key));

            if (element != null)
            {
                value = element.Value;
                return true;
            }

            value = default;
            return false;
        }

        public void Add(TKey key, TValue value)
        {
            if (Contains(key))
            {
                throw new ArgumentException(nameof(key));
            }

            var newElement = new KeyValueElement<TKey, TValue>(key, value);

            _elements.Add(newElement);
        }

        public IEnumerator<KeyValueElement<TKey, TValue>> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}