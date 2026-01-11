namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Check is null or empty")]
    public class StringIsNullOrEmptyNode : OperationStringNode<bool>
    {
        protected override bool Operation(string input)
        {
            return string.IsNullOrEmpty(input);
        }
    }
}
