using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Trim")]
    public class TrimStringNode : OperationStringNode<string>
    {
        [SerializeField] private TrimStringType _trimType;
        protected override string Operation(string input)
        {
            switch (_trimType)
            {
                case TrimStringType.All:
                    return input.Trim();
                case TrimStringType.Start:
                    return input.TrimStart();
                case TrimStringType.End:
                    return input.TrimEnd();
            }

            return input;
        }
    }
}
