using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Get Index From String By Index")]
    public class GetIndexFromStringByIndex : OperationStringNode<int>
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private string _value;

        [Input(connectionType = ConnectionType.Override), SerializeField] private int _startIndex;
        protected override int Operation(string input)
        {
            string value = _value;

            int startIndex = _startIndex;

            if (GetInputPort(nameof(_value)).Connection != null)
            {
                value = GetDataFromPort<string>(nameof(_value));
            }

            if (GetInputPort(nameof(_startIndex)).Connection != null)
            {
                _startIndex = GetDataFromPort<int>(nameof(_startIndex));
            }

            return input.IndexOf(value, startIndex);
        }
    }
}
