using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Parsers
{
    public class ParseFloatNode : ParseNode<string, float>
    {
        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }

            string input = GetInputObject() as string;

            return float.Parse(input);
        }
    }
}
