using System;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Split")]
    public class SplitStringNode : OperationStringNode<string[]>
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private string _splitData;

        [SerializeField] private SplitOptions _options;

        protected override string[] Operation(string input)
        {
            string splitData = _splitData;

            var inputSplitData = GetInputPort(nameof(_splitData));

            if (inputSplitData.Connection != null)
            {
                splitData = GetDataFromPort<string>(nameof(_splitData));
            }


            StringSplitOptions options = _options == SplitOptions.None ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries;

            return input.Split(splitData, options);
        }
    }
}
