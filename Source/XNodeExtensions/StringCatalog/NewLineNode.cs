using System;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Get New Line")]
    public class NewLineNode : BaseNode
    {
        [Output(ShowBackingValue.Never), SerializeField] private string _line;

        public override object GetValue(NodePort port)
        {
            return Environment.NewLine;
        }
    }
}
