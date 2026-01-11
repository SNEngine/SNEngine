using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Parsers
{
    public class ParseUintNode : ParseNode<string, uint>
    {
        public override object GetValue(NodePort port)
        {
            string input = GetInputObject() as string;

            return uint.Parse(input);
        }
    }
}
