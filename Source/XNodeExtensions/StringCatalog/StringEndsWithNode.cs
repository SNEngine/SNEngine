using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/String Ends With")]
    public class StringEndsWithNode : OperationStringNode<bool>
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private string _value;
        protected override bool Operation(string input)
        {
            string argument = _value;

            if (GetInputPort(nameof(_value)).Connection != null)
            {
                argument = GetDataFromPort<string>(nameof(_value));
            }

            return input.EndsWith(argument);
        }
    }
}
