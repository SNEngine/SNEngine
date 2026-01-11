using System.Numerics;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Parsers
{
    public class ParseBigIntegerNode : ParseNode<string, BigInteger>
    {
        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }
            string input = GetInputObject() as string;

            return BigInteger.Parse(input);
        }
    }
}
