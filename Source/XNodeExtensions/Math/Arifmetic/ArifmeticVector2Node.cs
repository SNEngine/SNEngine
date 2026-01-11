using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Math.Arifmetic
{
    [NodeTint("#494a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Math/Arifmetic/Vector2")]
    public class ArifmeticVector2Node : ArifmeticNode<Vector2>
    {
        protected override Vector2 Decrement(Vector2 a, Vector2 b)
        {
            return a - b;
        }

        protected override Vector2 Divide(Vector2 a, Vector2 b)
        {
            return a / b;
        }

        protected override Vector2 Increment(Vector2 a, Vector2 b)
        {
            return a + b;
        }

        protected override Vector2 Multiply(Vector2 a, Vector2 b)
        {
            return a * b;
        }

        protected override Vector2 Percent(Vector2 a, Vector2 b)
        {
            float xA = a.x;

            float yA = a.y;

            float xB = b.x;

            float yB = b.y;

            float x = xA % xB;

            float y = yA % yB;

            return new Vector2(x, y);
        }
    }
}
