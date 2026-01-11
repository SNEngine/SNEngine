using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.UnityApplication
{
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Unity API/Application/Get Build GUID")]
    public class GetApplicationBuildGUIDNode : BaseNode
    {
        [Output(ShowBackingValue.Never), SerializeField] private string _guid;

        public override object GetValue(NodePort port)
        {
            return Application.buildGUID;
        }
    }
}
