using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Insert")]
    public class InsertStringNode : OperationStringNode<string>
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private int _startIndex;

        [Input(connectionType = ConnectionType.Override), SerializeField] private string _value;
        protected override string Operation(string input)
        {
            int startIndex = _startIndex;

            string value = _value;

            if (GetInputPort(nameof(_startIndex)).Connection != null)
            {
                startIndex = GetDataFromPort<int>(nameof(_startIndex));
            }

            if (GetInputPort(nameof(_value)).Connection != null)
            {
                _value = GetDataFromPort<string>(nameof(_value));
            }

            return input.Insert(startIndex, value);
        }
    }
}
