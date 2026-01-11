using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.UnityApplication
{
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Unity API/Application/Get Version")]
    public class GetVersionApplicationNode : BaseNode
    {
        [Output(ShowBackingValue.Never), SerializeField] private string _version;

        public override object GetValue(NodePort port)
        {
            return Application.version;
        }
    }
}
