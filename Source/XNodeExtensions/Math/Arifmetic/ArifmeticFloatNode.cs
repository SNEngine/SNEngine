namespace SiphoinUnityHelpers.XNodeExtensions.Math.Arifmetic
{
    [NodeTint("#524949")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Math/Arifmetic/Float")]
    public class ArifmeticFloatNode : ArifmeticNode<float>
    {
        protected override float Decrement(float a, float b)
        {
            return a - b;
        }

        protected override float Divide(float a, float b)
        {
            return a / b;
        }

        protected override float Increment(float a, float b)
        {
            return a + b;
        }

        protected override float Multiply(float a, float b)
        {
            return a * b;
        }

        protected override float Percent(float a, float b)
        {
            return a % b;
        }
    }
}
