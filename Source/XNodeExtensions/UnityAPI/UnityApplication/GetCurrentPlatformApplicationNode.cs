using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.UnityApplication
{
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Unity API/Application/Get Current Runtime Platform")]
    public class GetCurrentPlatformApplicationNode : BaseNode
    {
        [Output(ShowBackingValue.Never), SerializeField] private RuntimePlatform _platform;

        public override object GetValue(NodePort port)
        {
            return Application.platform;
        }
    }
}
