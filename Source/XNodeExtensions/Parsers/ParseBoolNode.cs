using UnityEngine;
using XNode;
using System;
namespace SiphoinUnityHelpers.XNodeExtensions.Parsers
{
    public class ParseBoolNode : ParseNode<string, bool>
    {
        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }

            string input = GetInputObject() as string;

            if (int.TryParse(input, out int value))
            {
                string parseString = string.Empty;

                switch (value)
                {
                    case 0:
                        parseString = "False";
                        break;
                        case 1:
                        parseString = "True";
                        break;
                    default:
                        throw new ArgumentException("not valid number for convert to Bool");
                }

                return bool.Parse(parseString);
            }

            return bool.Parse(input);
        }
    }
}
