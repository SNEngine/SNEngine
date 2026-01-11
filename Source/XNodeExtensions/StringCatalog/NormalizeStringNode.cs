namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Normalize")]
    public class NormalizeStringNode : OperationStringNode<string>
    {
        protected override string Operation(string input)
        {
            return input.Normalize();
        }
    }
}
