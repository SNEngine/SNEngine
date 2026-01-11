using System;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions
{
    [NodeTint("#5c2b2b")]
    public class ExitNode : BaseNodeInteraction
    {
        public event EventHandler OnExit;

        public override void Execute()
        {
            OnExit?.Invoke(this, new ExitEventArgs());
        }

        public override bool CanSkip()
        {
            return false;
        }


    }
}
