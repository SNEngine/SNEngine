#if UNITY_EDITOR
using XNodeEditor;
using SiphoinUnityHelpers.XNodeExtensions.Variables.Set;
using SiphoinUnityHelpers.XNodeExtensions.Variables;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    [CustomNodeEditor(typeof(SetVariableNode<>))]
    public class SetVariableNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            XNodeEditorHelpers.DrawSetVaritableBody(this, serializedObject);
        }
    }

    // Базовые типы
    [CustomNodeEditor(typeof(SetIntNode))] public class SetIntNodeEditor : SetVariableNodeEditor { }
    [CustomNodeEditor(typeof(SetUintNode))] public class SetUintNodeEditor : SetVariableNodeEditor { }
    [CustomNodeEditor(typeof(SetStringNode))] public class SetStringNodeEditor : SetVariableNodeEditor { }
    [CustomNodeEditor(typeof(SetFloatNode))] public class SetFloatNodeEditor : SetVariableNodeEditor { }
    [CustomNodeEditor(typeof(SetDoubleNode))] public class SetDoubleNodeEditor : SetVariableNodeEditor { }
    [CustomNodeEditor(typeof(SetBoolNode))] public class SetBoolNodeEditor : SetVariableNodeEditor { }
    [CustomNodeEditor(typeof(SetColorNode))] public class SetColorNodeEditor : SetVariableNodeEditor { }
    [CustomNodeEditor(typeof(SetVector2Node))] public class SetVector2NodeEditor : SetVariableNodeEditor { }
    [CustomNodeEditor(typeof(SetQuaternionNode))] public class SetQuaternionNodeEditor : SetVariableNodeEditor { }

    // Unity-типы
    [CustomNodeEditor(typeof(SetTransformNode))] public class SetTransformNodeEditor : SetVariableNodeEditor { }
    [CustomNodeEditor(typeof(SetUlongNode))] public class SetUlongNodeEditor : SetVariableNodeEditor { }


    // Коллекции
    [CustomNodeEditor(typeof(IntNode))] public class SetIntCollectionNodeEditor : SetVariableNodeEditor { }
    [CustomNodeEditor(typeof(UintNode))] public class SetUintCollectionNodeEditor : SetVariableNodeEditor { }
    [CustomNodeEditor(typeof(StringNode))] public class SetStringCollectionNodeEditor : SetVariableNodeEditor { }
    [CustomNodeEditor(typeof(FloatNode))] public class SetFloatCollectionNodeEditor : SetVariableNodeEditor { }
    [CustomNodeEditor(typeof(DoubleNode))] public class SetDoubleCollectionNodeEditor : SetVariableNodeEditor { }
    [CustomNodeEditor(typeof(BoolNode))] public class SetBoolCollectionNodeEditor : SetVariableNodeEditor { }
    [CustomNodeEditor(typeof(ColorNode))] public class SetColorCollectionNodeEditor : SetVariableNodeEditor { }
    [CustomNodeEditor(typeof(Vector2Node))] public class SetVector2CollectionNodeEditor : SetVariableNodeEditor { }
    [CustomNodeEditor(typeof(QuaternionNode))] public class SetQuaternionCollectionNodeEditor : SetVariableNodeEditor { }
}
#endif