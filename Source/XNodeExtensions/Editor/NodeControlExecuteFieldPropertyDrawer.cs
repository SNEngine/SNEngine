using SiphoinUnityHelpers.XNodeExtensions;
using SiphoinUnityHelpers.XNodeExtensions.Attributes;
using SiphoinUnityHelpers.XNodeExtensions.Interfaces;
using SiphoinUnityHelpers.XNodeExtensions.Loop;
using SiphoinUnityHelpers.XNodeExtensions.NodesControlExecutes;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomPropertyDrawer(typeof(NodeControlExecuteFieldAttribute))]
public class NodeControlExecuteFieldPropertyDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        var node = property.serializedObject.targetObject as BaseNodeInteraction;

         if (node.Enter.Connection != null)
         {
            var connectedNode = node.Enter.Connection.node;

            if (connectedNode is ILoopNode)
            {
                var loopNode = connectedNode as ILoopNode;

                if (loopNode.NodeContainsOnLoop(node))
                {
                    DrawLabel(position, label, node, connectedNode);

                    return;
                }
                
            }

            if (connectedNode is IfNode)
            {
                var ifNode = connectedNode as IfNode;

                if (ifNode.NodeContainsOnBranch(node))
                {
                    DrawLabel(position, label, node, connectedNode);

                    return;
                }

            }

            if (connectedNode is GroupCallsNode)
            {
                var groupCallsNode = connectedNode as GroupCallsNode;

                if (groupCallsNode.NodeContainsOnOperation(node))
                {
                    DrawLabel(position, label, node, connectedNode);

                    return;
                }

            }
        }
        EditorGUI.PropertyField(position, property, label);
    }

    private static void DrawLabel(Rect position, GUIContent label, BaseNodeInteraction node, Node connectedNode)
    {
        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);

        style.fontSize = 10;

        EditorGUILayout.LabelField(label.text, $"Control by {NodeEditorUtilities.NodeDefaultName(connectedNode.GetType())}", style);

        Rect positionGuid = position;

        positionGuid.y += 20;

        style.fontSize = 9;

        node.SetEnable(false);
    }

}
