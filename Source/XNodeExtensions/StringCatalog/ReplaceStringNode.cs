using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Replace")]
    public class ReplaceStringNode : OperationStringNode<string>
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private string _oldValue;

        [Input(connectionType = ConnectionType.Override), SerializeField] private string _newValue;

        protected override string Operation(string input)
        {
            string newValue = _newValue;

            string oldValue = _oldValue;

            if (GetInputPort(nameof(_newValue)).Connection != null)
            {
                newValue = GetDataFromPort<string>(nameof(_newValue));
            }

            if (GetInputPort(nameof(_oldValue)).Connection != null)
            {
                oldValue = GetDataFromPort<string>(nameof(_oldValue));
            }

            return input.Replace(oldValue, newValue);
        }
    }
}
