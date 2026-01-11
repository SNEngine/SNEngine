using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.UnityAPI.UnityApplication
{
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/Unity API/Application/Get Persistent Data Path")]
    public class GetPersistentDataPathApplicationNode : BaseNode
    {
        [Output(ShowBackingValue.Never), SerializeField] private string _persistentDataPath;

        public override object GetValue(NodePort port)
        {
            return Application.persistentDataPath;
        }
    }
}
