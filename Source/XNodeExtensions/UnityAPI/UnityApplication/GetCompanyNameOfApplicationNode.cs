using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.UnityApplication
{
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Unity API/Application/Get Company Name")]
    public class GetCompanyNameOfApplicationNode : BaseNode
    {
        [Output(ShowBackingValue.Never), SerializeField] private string _companyName;

        public override object GetValue(NodePort port)
        {
            return Application.companyName;
        }
    }
}
