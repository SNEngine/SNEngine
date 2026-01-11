using System;
using System.Text.RegularExpressions;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions
{
    public static class RegexCollectionNode
    {
        public static int GetIndex (NodePort port)
        {
            if (port is null)
            {
                throw new ArgumentNullException("port");
            }

            if (!port.IsDynamic)
            {
                throw new ArgumentException("port not dynamic");
            }

            string indexFromPort = Regex.Replace(port.fieldName, "[^0-9]", string.Empty);

            int index = int.Parse(indexFromPort);

            return index;


        }
    }
}
