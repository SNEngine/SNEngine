namespace SiphoinUnityHelpers.XNodeExtensions.Math.Arifmetic
{
    [NodeTint("#524a4a")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Math/Arifmetic/Int")]
    public class ArifmeticIntNode : ArifmeticNode<int>
    {
        protected override int Decrement(int a, int b)
        {
            return a - b;
        }

        protected override int Divide(int a, int b)
        {
            return a / b;
        }

        protected override int Increment(int a, int b)
        {
            return a + b;
        }

        protected override int Multiply(int a, int b)
        {
            return a * b;
        }

        protected override int Percent(int a, int b)
        {
            return a % b;
        }
    }
}
