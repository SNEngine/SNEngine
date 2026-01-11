using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Parsers
{
    public class ParseUlongNode : ParseNode<string, ulong>
    {
        public override object GetValue(NodePort port)
        {
            string input = GetInputObject() as string;

            return ulong.Parse(input);
        }
    }
}
