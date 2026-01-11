using Cysharp.Threading.Tasks;
using SiphoinUnityHelpers.XNodeExtensions;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;
using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using SNEngine.Attributes;
using SNEngine.Debugging;
using SNEngine.Localization;
using SNEngine.SaveSystem;
using SNEngine.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;
namespace SNEngine.SelectVariantsSystem
{
    
    public class ShowVariantsNode : AsyncNode, ILocalizationNode, ISaveProgressNode
    {

        [SerializeField, Input(dynamicPortList = true, connectionType = ConnectionType.Override), ReadOnly(ReadOnlyMode.OnEditor)] private string[] _variants = new string[]
        {
            "Variant A",
            "Variant B"
        };

        private string[] _currentVariants;
        [Header("Parameters:")]

        [Space]

        [Input(connectionType = ConnectionType.Override), SerializeField, LeftToggle] private bool _hideCharacters = true;

        [Input(connectionType = ConnectionType.Override), SerializeField, LeftToggle] private bool _hideDialogWindow = true;

        [Input(connectionType = ConnectionType.Override), SerializeField, LeftToggle] private bool _returnCharacterVisible = true;

        [Space]

        [SerializeField] private AnimationButtonsType _typeAnimation = AnimationButtonsType.Fade;

        [Header("User selected: (start with 0)")]

        [Space]

        [Output(ShowBackingValue.Never), SerializeField] private int _selectedIndex;

        private int? _index;
        private bool _selected;

        public override void Execute()
        {
            if (_currentVariants is null || _currentVariants.Length == 0)
            {
                _currentVariants = _variants;
            }
            base.Execute();

            Show().Forget();


        }

        public override object GetValue(NodePort port)
        {
            return _index;
        }

        private async UniTask Show()
        {
            if (_selected)
            {
                StopTask();
                return;
            }
            _index = null;

            var sourceVariants = _currentVariants ?? _variants;
            var variants = sourceVariants.ToArray();

            bool hideDialogWindow = _hideDialogWindow;
            bool hideCharacters = _hideCharacters;
            bool returnCharacterVisible = _returnCharacterVisible;

            var inputHideDialogWindow = GetInputPort(nameof(_hideDialogWindow));
            var inputHideCharacters = GetInputPort(nameof(_hideCharacters));
            var inputReturnCharacterVisible = GetInputPort(nameof(_returnCharacterVisible));

            if (inputHideCharacters.Connection != null)
            {
                hideCharacters = GetDataFromPort<bool>(nameof(_hideCharacters));
            }

            if (inputHideDialogWindow.Connection != null)
            {
                hideDialogWindow = GetDataFromPort<bool>(nameof(_hideDialogWindow));
            }

            if (inputReturnCharacterVisible.Connection != null)
            {
                returnCharacterVisible = GetDataFromPort<bool>(nameof(_returnCharacterVisible));
            }

            for (int i = 0; i < variants.Length; i++)
            {
                variants[i] = TextParser.ParseWithProperties(variants[i], graph as BaseGraph);
            }

            var serviceShowVariants = NovelGame.Instance.GetService<SelectVariantsService>();

            serviceShowVariants.OnSelect += OnSelect;

            serviceShowVariants.ShowVariants(variants, hideCharacters, hideDialogWindow, returnCharacterVisible, _typeAnimation);

            while (_index == null)
            {
                await UniTask.WaitUntil(() => _index != null, cancellationToken: TokenSource.Token);
            }
        }

        private void OnSelect(int index)
        {
            var serviceShowVariants = NovelGame.Instance.GetService<SelectVariantsService>();

            serviceShowVariants.OnSelect -= OnSelect;

            _index = index;

            StopTask();

        }

        public override void SkipWait()
        {
            base.StopTask();
        }

        public override bool CanSkip()
        {
            return false;
        }
        #region Localization
        public object GetOriginalValue()
        {
            return _variants.AsEnumerable();
        }

        public void SetValue(object value)
        {
            if (value is IEnumerable<object> objectsEnumerable)
            {
                if (objectsEnumerable.All(x => x is string))
                {
                    List<string> strings = objectsEnumerable.Cast<string>().ToList();
                    _currentVariants = strings.ToArray();
                }
                else
                {
                    NovelGameDebug.LogError($"Error SetValue for node {GetType().Name} GUID {GUID}: list contains non-string elements");
                }
            }

            else if (value is IEnumerable<string> stringsEnumerable)
            {
                _currentVariants = stringsEnumerable.ToArray();
            }
            else
            {
                NovelGameDebug.LogError($"Error SetValue for node {GetType().Name} GUID {GUID}: value is not a List<object>");
            }
        }


        public object GetValue()
        {
            return _variants.AsEnumerable();
        }
        #endregion

        #region Save
        public object GetDataForSave()
        {
            return _index;
        }

        public void SetDataFromSave(object data)
        {
            if (data is null)
            {
                return;
            }
            if (data is long integer)
            {
                if (integer > -1)
                {
                    _index = (int)integer;
                    _selected = true;
                }
            }

            else
            {
                NovelGameDebug.LogError($"data for Show Variants Node is invalid: Type data: {data.GetType().Name}");
            }
        }

        public void ResetSaveBehaviour()
        {
            _selected = false;
        }
        #endregion
    }
}
