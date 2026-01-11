namespace SiphoinUnityHelpers.XNodeExtensions.Random
{
    [NodeTint("#6b3d5c")]
    public class RandomUintNode : RandomRangeNode<uint>
    {
        protected override uint Parse(string data)
        {
            return uint.Parse(data);
        }

        protected override uint Range(uint min, uint max)
        {
            int minInt = (int)min;

            int maxInt = (int)max;

            return (uint)UnityEngine.Random.Range(minInt, maxInt + 1);
        }
    }
}
