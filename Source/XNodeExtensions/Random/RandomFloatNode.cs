namespace SiphoinUnityHelpers.XNodeExtensions.Random
{
    [NodeTint("#6b3d5c")]
    public class RandomFloatNode : RandomRangeNode<float>
    {
        protected override float Parse(string data)
        {
            return float.Parse(data);
        }

        protected override float Range(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
}
