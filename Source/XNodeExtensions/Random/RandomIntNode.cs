namespace SiphoinUnityHelpers.XNodeExtensions.Random
{
    [NodeTint("#6b3d5c")]
    public class RandomIntNode : RandomRangeNode<int>
    {
        protected override int Parse(string data)
        {
            return int.Parse(data);
        }

        protected override int Range(int min, int max)
        {
            return UnityEngine.Random.Range(min, max + 1);
        }
    }
}
