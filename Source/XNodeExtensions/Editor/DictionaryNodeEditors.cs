#if UNITY_EDITOR
using XNodeEditor;
using SiphoinUnityHelpers.XNodeExtensions.Variables.DictionarySystem.Get;
using SiphoinUnityHelpers.XNodeExtensions.Variables.DictionarySystem.Set;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    
    public abstract class GetDictionaryNodeEditor : NodeEditor
    {
        public override void OnBodyGUI() => XNodeEditorHelpers.DrawGetDictionaryBody(this, serializedObject);


    }
    public abstract class SetDictionaryNodeEditor : NodeEditor
    {
        public override void OnBodyGUI() => XNodeEditorHelpers.DrawSetDictionaryBody(this, serializedObject);
    }

    [CustomNodeEditor(typeof(GetStringStringDictionaryNode))]
    public class GetStringStringDictionaryNodeEditor : GetDictionaryNodeEditor { }

    [CustomNodeEditor(typeof(GetStringIntDictionaryNode))]
    public class GetStringIntDictionaryNodeEditor : GetDictionaryNodeEditor { }

    [CustomNodeEditor(typeof(SetStringStringDictionaryNode))]
    public class SetStringStringDictionaryNodeEditor : SetDictionaryNodeEditor { }

    [CustomNodeEditor(typeof(SetStringIntDictionaryNode))]
    public class SetStringIntDictionaryNodeEditor : SetDictionaryNodeEditor { }
}
#endif