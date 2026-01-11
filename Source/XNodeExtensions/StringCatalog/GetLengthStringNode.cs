namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Get Length")]
    public class GetLengthStringNode : OperationStringNode<int>
    {
        protected override int Operation(string input)
        {
            return input.Length;
        }
    }
}
