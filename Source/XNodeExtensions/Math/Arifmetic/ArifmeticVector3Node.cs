using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.Math.Arifmetic
{
    [NodeTint("#515249")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Math/Arifmetic/Vector3")]
    public class ArifmeticVector3Node : ArifmeticNode<Vector3>
    {
        protected override Vector3 Decrement(Vector3 a, Vector3 b)
        {
            return a - b;
        }

        protected override Vector3 Divide(Vector3 a, Vector3 b)
        {
            float xA = a.x;

            float yA = a.y;

            float xB = b.x;

            float yB = b.y;

            float zA = a.z;

            float zB = b.z;

            float x = xA / xB;

            float y = yA / yB;

            float z = zA / zB;

            return new Vector3(x, y, z);
        }

        protected override Vector3 Increment(Vector3 a, Vector3 b)
        {
            return a + b;
        }

        protected override Vector3 Multiply(Vector3 a, Vector3 b)
        {
            float xA = a.x;

            float yA = a.y;

            float xB = b.x;

            float yB = b.y;

            float zA = a.z;

            float zB = b.z;

            float x = xA * xB;

            float y = yA * yB;

            float z = zA * zB;

            return new Vector3(x, y, z);
        }

        protected override Vector3 Percent(Vector3 a, Vector3 b)
        {
            float xA = a.x;

            float yA = a.y;

            float xB = b.x;

            float yB = b.y;

            float zA = a.z;

            float zB = b.z;

            float x = xA % xB;

            float y = yA % yB;

            float z = zA % zB;

            return new Vector3(x, y, z);
        }
    }
}
