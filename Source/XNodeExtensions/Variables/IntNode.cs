using System;
using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables
{
    [NodeTint("#524a4a")]
    public class IntNode : VariableNode<int>
    {
        public override void SetValue(object value)
        {
            if (value is long longValue)
            {
                SetValue((int)longValue);
                return;
            }

            base.SetValue(value);
        }
    }
}