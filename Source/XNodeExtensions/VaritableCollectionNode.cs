using Newtonsoft.Json.Linq;
using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using SiphoinUnityHelpers.XNodeExtensions.Debugging;
using SiphoinUnityHelpers.XNodeExtensions.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions
{
    public abstract class VariableCollectionNode<T> : VariableNode, IList<T>, IList
    {
        [SerializeField, HideInInspector]
        private List<T> _startValue = new List<T>();

        [Space(10)]
        [SerializeField, Output(ShowBackingValue.Always, dynamicPortList = true), ReadOnly(ReadOnlyMode.OnEditor)]
        private List<T> _elements = new List<T>();

        [Space(10)]
        [SerializeField, Output(ShowBackingValue.Never), ReadOnly(ReadOnlyMode.OnEditor)]
        private NodePortEnumerable _enumerable;

        public int Count => _elements.Count;
        public bool IsReadOnly => false;
        public bool IsFixedSize => false;
        public object SyncRoot => ((ICollection)_elements).SyncRoot;
        public bool IsSynchronized => ((ICollection)_elements).IsSynchronized;

        object IList.this[int index]
        {
            get => (index >= 0 && index < _elements.Count) ? _elements[index] : null;
            set => _elements[index] = (T)value;
        }

        public T this[int index]
        {
            get => _elements[index];
            set => _elements[index] = value;
        }

        public override object GetStartValue() => _startValue;

        public override object GetValue(NodePort port)
        {
            if (port.fieldName != nameof(_enumerable))
            {
                int index = RegexCollectionNode.GetIndex(port);
                if (index >= 0 && index < _elements.Count)
                {
                    return _elements[index];
                }
                return null;
            }

            return _elements.AsEnumerable();
        }

        public void SetValue(int index, T value)
        {
            if (index < 0) return;

            if (index >= _elements.Count)
            {
                int countToAdd = index - _elements.Count + 1;
                for (int i = 0; i < countToAdd; i++)
                {
                    _elements.Add(default);
                }
#if UNITY_EDITOR
                UpdatePorts();
#endif
            }

            _elements[index] = value;
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < _elements.Count)
            {
                _elements.RemoveAt(index);
#if UNITY_EDITOR
                UpdatePorts();
#endif
            }
        }

        public void SetValue(IEnumerable<T> value)
        {
            _elements = value.ToList();
#if UNITY_EDITOR
            UpdatePorts();
#endif
        }

        public override void SetValue(object value)
        {
            if (value is IEnumerable<T> collection)
            {
                _elements = collection.ToList();
#if UNITY_EDITOR
                UpdatePorts();
#endif
            }
            else
            {
                XNodeExtensionsDebug.LogError($"Collection node {GUID} don`t apply the value {value.GetType().Name}");
            }
        }

        public void Add(T item)
        {
            _elements.Add(item);
#if UNITY_EDITOR
            UpdatePorts();
#endif
        }

        public int Add(object value)
        {
            _elements.Add((T)value);
#if UNITY_EDITOR
            UpdatePorts();
#endif
            return _elements.Count - 1;
        }

        public void Clear()
        {
            _elements.Clear();
#if UNITY_EDITOR
            UpdatePorts();
#endif
        }

        public bool Remove(T item)
        {
            bool result = _elements.Remove(item);
#if UNITY_EDITOR
            if (result) UpdatePorts();
#endif
            return result;
        }

        public void Insert(int index, T item)
        {
            _elements.Insert(index, item);
#if UNITY_EDITOR
            UpdatePorts();
#endif
        }

        public override object GetCurrentValue() => _elements.ToList();

        public override void ResetValue()
        {
            _elements = _startValue != null ? _startValue.ToList() : new List<T>();
#if UNITY_EDITOR
            UpdatePorts();
#endif
        }

        public void Remove(object value) => Remove((T)value);
        public bool Contains(T item) => _elements.Contains(item);
        public bool Contains(object value) => _elements.Contains((T)value);
        public int IndexOf(T item) => _elements.IndexOf(item);
        public int IndexOf(object value) => _elements.IndexOf((T)value);
        public void Insert(int index, object value) => Insert(index, (T)value);
        public void CopyTo(T[] array, int arrayIndex) => _elements.CopyTo(array, arrayIndex);
        public void CopyTo(Array array, int index) => ((ICollection)_elements).CopyTo(array, index);
        public IEnumerator<T> GetEnumerator() => _elements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            Validate();
        }

        protected override void Validate()
        {
            base.Validate();
            if (!Application.isPlaying)
            {
                _startValue = _elements.ToList();
                UpdatePorts();
            }
        }

        protected override void ValidateName()
        {
            name = Color.ToColorTag($"{Name} ({GetDefaultName()}List)");
        }
#endif
    }
}