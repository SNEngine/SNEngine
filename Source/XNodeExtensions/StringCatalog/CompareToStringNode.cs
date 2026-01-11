using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Compare To")]
    public class CompareToStringNode<CompareType> : OperationStringNode<int>
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private CompareType _target;
        protected override int Operation(string input)
        {
            CompareType argument = _target;

            if (GetInputPort(nameof(_target)).Connection != null)
            {
                argument = GetDataFromPort<CompareType>(nameof(_target));
            }

            return input.CompareTo(argument);
        }
    }
}
