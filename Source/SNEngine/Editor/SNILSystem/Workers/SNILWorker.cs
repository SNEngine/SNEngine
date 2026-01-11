using System.Collections.Generic;
using SiphoinUnityHelpers.XNodeExtensions;

namespace SNEngine.Editor.SNILSystem.Workers
{
    public abstract class SNILWorker
    {
        public abstract void ApplyParameters(BaseNode node, Dictionary<string, string> parameters);
    }
}