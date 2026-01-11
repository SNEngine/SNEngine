namespace SiphoinUnityHelpers.XNodeExtensions.Math.Arifmetic
{
    [NodeTint("#524a4a")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Math/Arifmetic/Ulong")]
    public class ArifmeticulongNode : ArifmeticNode<ulong>
    {
        protected override ulong Decrement(ulong a, ulong b)
        {
            return a - b;
        }

        protected override ulong Divide(ulong a, ulong b)
        {
            return a / b;
        }

        protected override ulong Increment(ulong a, ulong b)
        {
            return a + b;
        }

        protected override ulong Multiply(ulong a, ulong b)
        {
            return a * b;
        }

        protected override ulong Percent(ulong a, ulong b)
        {
            return a % b;
        }
    }
}
