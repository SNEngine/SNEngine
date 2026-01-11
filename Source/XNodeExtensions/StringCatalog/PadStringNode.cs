using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Pad")]
    internal class PadStringNode : OperationStringNode<string>
    {
        [Input(connectionType = ConnectionType.Override), SerializeField] private int _totalWidth;

        [SerializeField] private PadStringType _type;
        protected override string Operation(string input)
        {
            int totalWidth = _totalWidth;

            if (GetInputPort(nameof(_totalWidth)).Connection != null)
            {
                totalWidth = GetDataFromPort<int>(nameof(_totalWidth));
            }

            return _type == PadStringType.Right ? input.PadRight(totalWidth) : input.PadLeft(totalWidth);
        }
    }
}
