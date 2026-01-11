using SiphoinUnityHelpers.XNodeExtensions.Debugging;
using UnityEngine;

namespace SiphoinUnityHelpers.XNodeExtensions
{
    [NodeTint("#2b335c")]
    public class StartNode : BaseNodeInteraction
    {
        public override void Execute()
        {
            XNodeExtensionsDebug.Log($"node queue from graph {graph.name} started");
        }
        public override bool CanSkip()
        {
            return false;
        }

        
    }
}
