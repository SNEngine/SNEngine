namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Normalize")]
    public class StringIsNormalized : OperationStringNode<bool>
    {
        protected override bool Operation(string input)
        {
            return input.IsNormalized();
        }
    }
}
