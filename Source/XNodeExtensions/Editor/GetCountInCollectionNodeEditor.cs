using SiphoinUnityHelpers.XNodeExtensions.Variables.Collection;
using XNodeEditor;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    [CustomNodeEditor(typeof(GetCountInCollectionNode))]
    internal class GetCountInCollectionNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            XNodeEditorHelpers.DrawGetCountInCollectionBody(this, serializedObject);
        }
    }
}
