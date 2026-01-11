using UnityEngine;
using XNode;
using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using SiphoinUnityHelpers.XNodeExtensions.Extensions;
using SiphoinUnityHelpers.XNodeExtensions.Debugging;
using SNEngine.Serialization;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SiphoinUnityHelpers.XNodeExtensions
{
    public abstract class VariableNode : BaseNode
    {
        [SerializeField, ReadOnly(ReadOnlyMode.OnEditor)] private string _name;

        [Space(25)]

        [SerializeField, ReadOnly(ReadOnlyMode.OnEditor)] private Color32 _color = UnityEngine.Color.white;

        public string Name { get => _name; set => _name = value; }
        public Color32 Color { get => _color; set => _color = value; }

        public abstract object GetStartValue();

        public abstract void ResetValue();

        public abstract object GetCurrentValue();

        public abstract void SetValue(object value);

        protected virtual void OnValueChanged(object oldValue, object newValue) { }

#if UNITY_EDITOR

        protected virtual void Validate()
        {
            if (!Application.isPlaying)
            {
                if (string.IsNullOrEmpty(Name))
                {
                    Name = $"{GetDefaultName()} Variable";
                }
            }
        }

        protected virtual void OnValidate()
        {
            ValidateName();
        }

        protected virtual void ValidateName()
        {
            name = Color.ToColorTag($"{Name} ({GetDefaultName()})");
        }

        protected virtual new void OnEnable()
        {
            base.OnEnable();

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                ResetValue();
            }
        }

#endif
    }

    public abstract class VariableNode<T> : VariableNode
    {
        private T _startValue;
        private T _editorOldValue;
        private bool _isInitialized;

        [SerializeField, Output(ShowBackingValue.Always), ReadOnly(ReadOnlyMode.OnEditor)] private T _value;

        public override object GetStartValue()
        {
            return _startValue;
        }

        public override object GetValue(NodePort port)
        {
            return _value;
        }

        public void SetValue(T value)
        {
            T oldValue = _value;
            _value = value;

            if (!EqualityComparer<T>.Default.Equals(oldValue, value))
            {
                OnValueChanged(oldValue, value);
                OnValueChanged((object)oldValue, (object)value);
            }
        }

        public override void SetValue(object value)
        {
            if (value is T targetValue)
            {
                SetValue(targetValue);
            }
            else
            {
                XNodeExtensionsDebug.LogError($"Variable node {GUID} not apply the value {value?.GetType().Name}");
            }
        }

        public override object GetCurrentValue()
        {
            return _value;
        }

        public override void ResetValue()
        {
            T oldValue = _value;
            _value = _startValue;

            if (!EqualityComparer<T>.Default.Equals(oldValue, _value))
            {
                OnValueChanged(oldValue, _value);
                OnValueChanged((object)oldValue, (object)_value);
            }
        }

        protected virtual void OnValueChanged(T oldValue, T newValue) { }

#if UNITY_EDITOR

        protected override void OnEnable()
        {
            base.OnEnable();
            _editorOldValue = _value;
            _isInitialized = true;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_isInitialized && !EqualityComparer<T>.Default.Equals(_editorOldValue, _value))
            {
                T old = _editorOldValue;
                _editorOldValue = _value;

                OnValueChanged(old, _value);
                OnValueChanged((object)old, (object)_value);
            }

            Validate();
        }

        protected override void Validate()
        {
            base.Validate();

            if (!Application.isPlaying)
            {
                _startValue = _value;
            }
        }
#endif
    }
}