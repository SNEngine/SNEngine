using System;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Contains")]
    public class ContainsStringNode : OperationStringNode<bool>
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private string _containsString;
        protected override bool Operation(string input)
        {
            string argument = _containsString;

            if (GetInputPort(nameof(_containsString)).Connection != null)
            {
                argument = GetDataFromPort<string>(nameof(_containsString));
            }

            return input.Contains(argument);
        }
    }
}
