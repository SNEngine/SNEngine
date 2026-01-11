using SiphoinUnityHelpers.XNodeExtensions.NodesControlExecutes.Switch;
using UnityEditor;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    public partial class SwitchIntNodeEditor
    {
        [CustomNodeEditor(typeof(SwitchFloatNode))]
        public class SwitchFloatNodeEditor : BaseSwitchNodeEditor<float>
        {
        }
    }
}