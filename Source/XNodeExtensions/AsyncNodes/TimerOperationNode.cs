using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions.AsyncNodes
{
    public abstract class TimerOperationNode : BaseNodeInteraction
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override), SerializeField] private TimerNode _timer;

        public override void Execute()
        {
            TimerNode timer = GetDataFromPort<TimerNode>(nameof(_timer));

            Operation(timer);

        }
        protected abstract void Operation(TimerNode timerNode);
    }
}
