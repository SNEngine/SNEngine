using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.UnityApplication
{
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Unity API/Application/Is Batch Mode")]
    public class ApplicationIsBatchModeNode : BaseNode
    {
        [Output(ShowBackingValue.Never), SerializeField] private bool _isBatchMode;

        public override object GetValue(NodePort port)
        {
            return Application.isBatchMode;
        }
    }
}
