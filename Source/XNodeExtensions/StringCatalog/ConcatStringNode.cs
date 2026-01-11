namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Concat")]
    public class ConcatStringNode : OperationABNode<string, string>
    {
        protected override string Operation(string a, string b)
        {
            return a + b;
        }
    }
}
