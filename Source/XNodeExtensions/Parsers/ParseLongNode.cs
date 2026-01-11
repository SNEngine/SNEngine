using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Parsers
{
    public class ParseLongNode : ParseNode<string, long>
    {
        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }

            string input = GetInputObject() as string;

            return long.Parse(input);
        }
    }
}
