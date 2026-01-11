using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Get Index From String")]
    public class GetIndexFromString : OperationStringNode<int>
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private string _value;
        protected override int Operation(string input)
        {
            string argument = _value;

            if (GetInputPort(nameof(_value)).Connection != null)
            {
                argument = GetDataFromPort<string>(nameof(_value));
            }

            return input.IndexOf(argument);
        }
    }
}
