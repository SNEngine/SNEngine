using UnityEngine;
using XNode;

namespace SiphoinUnityHelpers.XNodeExtensions.StringCatalog
{
    [NodeTint("#4c4a52")]
    [CreateNodeMenu("Siphoin Unity Helpers/X Node Extensions/String/Nameof")]
    public class NameOfNode : BaseNode
    {
        [Input(ShowBackingValue.Never, ConnectionType.Override)] private Object _object;

        [Output(ShowBackingValue.Never)] private string _nameof;
        public override object GetValue(NodePort port)
        {
            if (!Application.isPlaying)
            {
                return base.GetValue(port);
            }

            object target = GetDataFromPort<Object>(nameof(_object));

            return nameof(target);
        }
    }
}
