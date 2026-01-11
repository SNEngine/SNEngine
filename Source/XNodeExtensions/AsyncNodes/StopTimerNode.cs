namespace SiphoinUnityHelpers.XNodeExtensions.AsyncNodes
{
    [NodeTint("#5c2b3f")]
    public class StopTimerNode : TimerOperationNode
    {
        protected override void Operation(TimerNode timerNode)
        {
            timerNode?.Stop();
        }
    }
}
