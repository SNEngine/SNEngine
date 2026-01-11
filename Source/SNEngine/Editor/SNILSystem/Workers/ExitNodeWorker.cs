using System.Collections.Generic;
using SiphoinUnityHelpers.XNodeExtensions;

namespace SNEngine.Editor.SNILSystem.Workers
{
    public class ExitNodeWorker : SNILWorker
    {
        public override void ApplyParameters(BaseNode node, Dictionary<string, string> parameters)
        {
            // Exit node typically doesn't have parameters to set
            // Just ensure the node is properly configured as an exit node
        }
    }
}