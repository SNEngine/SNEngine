namespace SiphoinUnityHelpers.XNodeExtensions.Math.Arifmetic
{
    [NodeTint("#524a4a")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Math/Arifmetic/Uint")]
    public class ArifmeticuintNode : ArifmeticNode<uint>
    {
        
        protected override uint Decrement(uint a, uint b)
        {
            return a - b;
        }

        protected override uint Divide(uint a, uint b)
        {
            return a / b;
        }

        protected override uint Increment(uint a, uint b)
        {
            return a + b;
        }

        protected override uint Multiply(uint a, uint b)
        {
            return a * b;
        }

        protected override uint Percent(uint a, uint b)
        {
            return a % b;
        }
    }
}
