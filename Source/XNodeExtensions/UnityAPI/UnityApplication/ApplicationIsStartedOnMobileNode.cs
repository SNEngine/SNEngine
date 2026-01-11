using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.UnityApplication
{
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Unity API/Application/Is Started on Mobile")]
    public class ApplicationIsStartedOnMobileNode : BaseNode
    {
        [Output(ShowBackingValue.Never), SerializeField] private bool _isMobile;

        public override object GetValue(NodePort port)
        {
            return Application.isMobilePlatform;
        }
    }
}
