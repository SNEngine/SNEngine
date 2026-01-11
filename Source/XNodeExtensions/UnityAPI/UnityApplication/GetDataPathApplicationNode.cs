using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.UnityApplication
{
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Unity API/Application/Get Data Path")]
    public class GetDataPathApplicationNode : BaseNode
    {
        [Output(ShowBackingValue.Never), SerializeField] private string _dataPath;

        public override object GetValue(NodePort port)
        {
            return Application.dataPath;
        }
    }
}
