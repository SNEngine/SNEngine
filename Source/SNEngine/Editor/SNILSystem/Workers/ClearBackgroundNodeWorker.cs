using System.Collections.Generic;
using SiphoinUnityHelpers.XNodeExtensions;

namespace SNEngine.Editor.SNILSystem.Workers
{
    public class ClearBackgroundNodeWorker : SNILWorker
    {
        public override void ApplyParameters(BaseNode node, Dictionary<string, string> parameters)
        {
            // ClearBackgroundNode не имеет параметров для установки
            // Нода просто очищает фон при выполнении
        }
    }
}