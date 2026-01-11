#if UNITY_EDITOR
using XNodeEditor;
using SNEngine.GlobalVariables.DictionarySystem;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    public class GetGlobalDictionaryNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            XNodeEditorHelpers.DrawGetGlobalDictionaryBody(this, serializedObject);
        }
    }

    
    [CustomNodeEditor(typeof(GetGlobalStringIntDictionaryNode))]
    public class GetGlobalStringIntDictionaryNodeEditor : GetGlobalDictionaryNodeEditor { }

    [CustomNodeEditor(typeof(GetGlobalStringStringDictionaryNode))]
    public class GetGlobalStringStringDictionaryNodeEditor : GetGlobalDictionaryNodeEditor { }

}
#endif