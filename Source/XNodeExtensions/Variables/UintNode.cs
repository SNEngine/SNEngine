namespace SiphoinUnityHelpers.XNodeExtensions.Variables
{
    [NodeTint("#524a4a")]
    public class UintNode : VariableNode<uint>
    {
        public override void SetValue(object value)
        {
            if (value is long longValue)
            {
                SetValue((uint)longValue);
                return;
            }

            base.SetValue(value);
        }
    }
}