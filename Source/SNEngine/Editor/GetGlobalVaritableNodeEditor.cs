#if UNITY_EDITOR
using XNodeEditor;
using SNEngine.GlobalVariables;
using SNEngine.GlobalVariables.Textures;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    [CustomNodeEditor(typeof(GetGlobalVariableNode<>))]
    public class GetGlobalVaritableNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            XNodeEditorHelpers.DrawGetGlobalDictionaryBody(this, serializedObject);
        }
    }

    [CustomNodeEditor(typeof(GetGlobalIntNode))] public class GetGlobalIntNodeEditor : GetGlobalVaritableNodeEditor { }
    [CustomNodeEditor(typeof(GetGlobalUintNode))] public class GetGlobalUintNodeEditor : GetGlobalVaritableNodeEditor { }
    [CustomNodeEditor(typeof(GetGlobalLongNode))] public class GetGlobalLongNodeEditor : GetGlobalVaritableNodeEditor { }
    [CustomNodeEditor(typeof(GetGlobalUlongNode))] public class GetGlobalUlongNodeEditor : GetGlobalVaritableNodeEditor { }
    [CustomNodeEditor(typeof(GetGlobalStringNode))] public class GetGlobalStringNodeEditor : GetGlobalVaritableNodeEditor { }
    [CustomNodeEditor(typeof(GetGlobalFloatNode))] public class GetGlobalFloatNodeEditor : GetGlobalVaritableNodeEditor { }
    [CustomNodeEditor(typeof(GetGlobalDoubleNode))] public class GetGlobalDoubleNodeEditor : GetGlobalVaritableNodeEditor { }
    [CustomNodeEditor(typeof(GetGlobalBoolNode))] public class GetGlobalBoolNodeEditor : GetGlobalVaritableNodeEditor { }
    [CustomNodeEditor(typeof(GetGlobalColorNode))] public class GetGlobalColorNodeEditor : GetGlobalVaritableNodeEditor { }
    [CustomNodeEditor(typeof(GetGlobalVector2Node))] public class GetGlobalVector2NodeEditor : GetGlobalVaritableNodeEditor { }
    [CustomNodeEditor(typeof(GetGlobalQuaternionNode))] public class GetGlobalQuaternionNodeEditor : GetGlobalVaritableNodeEditor { }

    // Unity-типы
    [CustomNodeEditor(typeof(GetGlobalTexture2DNode))] public class GetGlobalTexture2DNodeEditor : GetGlobalVaritableNodeEditor { }
    [CustomNodeEditor(typeof(GetGlobalAudioClipNode))] public class GetGlobalAudioClipNodeEditor : GetGlobalVaritableNodeEditor { }
    [CustomNodeEditor(typeof(GetGlobalMaterialNode))] public class GetGlobalMaterialNodeEditor : GetGlobalVaritableNodeEditor { }
    [CustomNodeEditor(typeof(GetGlobalPrefabNode))] public class GetGlobalPrefabNodeEditor : GetGlobalVaritableNodeEditor { }
    [CustomNodeEditor(typeof(GetGlobalSpriteNode))] public class GetGlobalSpriteNodeEditor : GetGlobalVaritableNodeEditor { }
    [CustomNodeEditor(typeof(GetGlobalTransformNode))] public class GetGlobalTransformNodeEditor : GetGlobalVaritableNodeEditor { }
    [CustomNodeEditor(typeof(GetGlobalTextureNode))] public class GetGlobalTextureNodeEditor : GetGlobalVaritableNodeEditor { }

}
#endif