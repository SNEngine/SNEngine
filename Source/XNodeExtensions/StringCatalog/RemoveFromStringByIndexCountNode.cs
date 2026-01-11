using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Remove By Index And Count")]
    [NodeWidth(260)]
    internal class RemoveFromStringByIndexCountNode : OperationStringNode<string>
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private int _index;

        [Input(connectionType = ConnectionType.Override), SerializeField] private int _count;
        protected override string Operation(string input)
        {
            int index = _index;

            int count = _count;

            if (GetInputPort(nameof(_index)).Connection != null)
            {
                index = GetDataFromPort<int>(nameof(_index));
            }

            if (GetInputPort(nameof(_count)).Connection != null)
            {
                count = GetDataFromPort<int>(nameof(_count));
            }

            return input.Remove(index, count);
        }
    }
}
