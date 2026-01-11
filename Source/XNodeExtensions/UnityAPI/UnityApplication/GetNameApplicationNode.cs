using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.UnityApplication
{
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Unity API/Application/Get Name")]
    public class GetNameApplicationNode : BaseNode
    {
        [Output(ShowBackingValue.Never), SerializeField] private string _name;

        public override object GetValue(NodePort port)
        {
            return Application.productName;
        }
    }
}
