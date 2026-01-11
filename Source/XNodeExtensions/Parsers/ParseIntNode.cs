using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Parsers
{
    public class ParseIntNode : ParseNode<string, int>
    {
        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }

            string input = GetInputObject() as string;

            return int.Parse(input);
        }
    }
}
