#if UNITY_EDITOR
using SiphoinUnityHelpers.XNodeExtensions.Variables.Collection;
using SiphoinUnityHelpers.XNodeExtensions.Variables.Set;
using XNodeEditor;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    [CustomNodeEditor(typeof(RemoveFromCollectionNode))]
    public class RemoveAtVariableNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            XNodeEditorHelpers.DrawRemoveAtVariableBody(this, serializedObject);
        }
    }
}
#endif