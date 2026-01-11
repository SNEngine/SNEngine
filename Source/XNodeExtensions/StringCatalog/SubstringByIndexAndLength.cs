using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Substring By Index And Length")]
    internal class SubstringByIndexAndLengthNode : OperationStringNode<string>
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private int _index;

        [Input(connectionType = ConnectionType.Override), SerializeField] private int _length;
        protected override string Operation(string input)
        {
            int index = _index;

            int length = _length;

            if (GetInputPort(nameof(_index)).Connection != null)
            {
                index = GetDataFromPort<int>(nameof(_index));
            }

            if (GetInputPort(nameof(_length)).Connection != null)
            {
                length = GetDataFromPort<int>(nameof(_length));
            }

            return input.Substring(index, length);
        }
    }
}
