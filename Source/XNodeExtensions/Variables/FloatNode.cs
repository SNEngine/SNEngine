using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.Variables
{
    [NodeTint("#524949")]
    public class FloatNode : VariableNode<float>
    {
        public override void SetValue(object value)
        {
            if (value is double doubleValue)
            {
                SetValue((float)doubleValue);
                return;
            }

            base.SetValue(value);
        }
    }
}