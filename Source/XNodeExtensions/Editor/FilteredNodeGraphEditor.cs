using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using SiphoinUnityHelpers.XNodeExtensions;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    public abstract class FilteredNodeGraphEditor : NodeGraphEditor
    {
        protected virtual bool IsNodeTypeAllowed(Type nodeType)
        {
            return true;
        }

        public override void AddContextMenuItems(GenericMenu menu)
        {
            Vector2 pos =
                NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition);

            var filteredNodeTypes = NodeEditorReflection.nodeTypes
                .Where(IsNodeTypeAllowed)
                .OrderBy(type => GetNodeMenuOrder(type))
                .ToArray();

            for (int i = 0; i < filteredNodeTypes.Length; i++)
            {
                Type type = filteredNodeTypes[i];

                string path = GetNodeMenuName(type);
                if (string.IsNullOrEmpty(path)) continue;

                XNode.Node.DisallowMultipleNodesAttribute disallowAttrib;
                bool disallowed = false;
                if (NodeEditorUtilities.GetAttrib(type, out disallowAttrib))
                {
                    int typeCount = target.nodes.Count(x => x.GetType() == type);
                    if (typeCount >= disallowAttrib.max) disallowed = true;
                }

                if (disallowed)
                    menu.AddItem(new GUIContent(path), false, null);
                else
                    menu.AddItem(new GUIContent(path), false, () =>
                    {
                        XNode.Node node = CreateNode(type, pos);
                        NodeEditorWindow.current.AutoConnect(node);
                    });
            }

            if (filteredNodeTypes.Length == 0)
            {
                menu.AddDisabledItem(new GUIContent("No Available Nodes"));
            }

            menu.AddSeparator("");
            if (NodeEditorWindow.copyBuffer != null && NodeEditorWindow.copyBuffer.Length > 0)
                menu.AddItem(new GUIContent("Paste"), false, () =>
                    NodeEditorWindow.current.PasteNodes(pos));
            else
                menu.AddDisabledItem(new GUIContent("Paste"));
            menu.AddItem(new GUIContent("Preferences"), false, () =>
                NodeEditorReflection.OpenPreferences());
            menu.AddCustomContextMenuItems(target);
        }
    }
}