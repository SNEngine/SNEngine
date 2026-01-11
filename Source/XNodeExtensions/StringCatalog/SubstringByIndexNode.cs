using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Substring By Index")]
    internal class SubstringByIndexNode : OperationStringNode<string>
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private int _index;
        protected override string Operation(string input)
        {
            int index = _index;

            if (GetInputPort(nameof(_index)).Connection != null)
            {
                index = GetDataFromPort<int>(nameof(_index));
            }

            return input.Substring(index);
        }
    }
}
