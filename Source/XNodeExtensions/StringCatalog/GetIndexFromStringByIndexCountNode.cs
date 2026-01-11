using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Get Index From String By Index And Count")]
    [NodeWidth(260)]
    public class GetIndexFromStringByIndexCount : OperationStringNode<int>
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private string _value;

        [Input(connectionType = ConnectionType.Override), SerializeField] private int _startIndex;

        [Input(connectionType = ConnectionType.Override), SerializeField] private int _count;
        protected override int Operation(string input)
        {
            string value = _value;

            int startIndex = _startIndex;

            int count = _count;

            if (GetInputPort(nameof(_value)).Connection != null)
            {
                value = GetDataFromPort<string>(nameof(_value));
            }

            if (GetInputPort(nameof(_startIndex)).Connection != null)
            {
                _startIndex = GetDataFromPort<int>(nameof(_startIndex));
            }

            if (GetInputPort(nameof(_count)).Connection != null)
            {
                count = GetDataFromPort<int>(nameof(_count));
            }

            return input.IndexOf(value, startIndex, count);
        }
    }
}
