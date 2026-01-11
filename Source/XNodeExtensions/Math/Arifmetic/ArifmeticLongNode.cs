namespace SiphoinUnityHelpers.XNodeExtensions.Math.Arifmetic
{
    [NodeTint("#494d52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Math/Arifmetic/Long")]
    public class ArifmeticLongNode : ArifmeticNode<long>
    {

        protected override long Decrement(long a, long b)
        {
            return a - b;
        }

        protected override long Divide(long a, long b)
        {
            return a / b;
        }

        protected override long Increment(long a, long b)
        {
            return a + b;
        }

        protected override long Multiply(long a, long b)
        {
            return a * b;
        }

        protected override long Percent(long a, long b)
        {
            return a % b;
        }
    }
}
