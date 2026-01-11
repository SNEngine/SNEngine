using SNEngine.AsyncNodes;
using System.Threading;
namespace SiphoinUnityHelpers.XNodeExtensions.AsyncNodes
{
    [NodeTint("#5c572b")]
    public abstract class AsyncNode : BaseNodeInteraction, IIncludeWaitingNode
    {

        private CancellationTokenSource _tokenSource;

        protected CancellationTokenSource TokenSource => _tokenSource;

        public bool IsWorking =>  _tokenSource != null;

        public override void Execute()
        {
            _tokenSource = new CancellationTokenSource();
        }

        public void StopTask ()
        {
            _tokenSource?.Cancel();

            _tokenSource = null;
        }

        public virtual void SkipWait ()
        {
            StopTask();
        }


    }
}
