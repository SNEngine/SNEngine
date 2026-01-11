using SNEngine.Graphs;
using UnityEngine;
using XNodeEditor;
using UnityEditor;
using SiphoinUnityHelpers.XNodeExtensions;
using System.Linq;
using System;

namespace SNEngine.Editor
{
    [CustomNodeGraphEditor(typeof(DialogueGraph))]
    public class DialogueGraphEditor : NodeGraphEditor
    {
        public override void OnGUI()
        {
            DrawToolbarButtons();
            base.OnGUI();
        }

        private void DrawToolbarButtons()
        {
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Add Node", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                Vector2 graphPos = NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition);
                NodeSelectorWindow.Open((nodeType, position) => {
                    CreateNode(nodeType, position);
                }, graphPos);
            }

            if (GUILayout.Button("Search Nodes", EditorStyles.toolbarButton, GUILayout.Width(120)))
            {
                NodeSearchWindow.Open(target as BaseGraph);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
        }

        public override void AddContextMenuItems(GenericMenu menu)
        {
            Vector2 pos = NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition);

            var nodeTypes = NodeEditorReflection.nodeTypes
                .OrderBy(type => GetNodeMenuOrder(type))
                .ToArray();

            for (int i = 0; i < nodeTypes.Length; i++)
            {
                Type type = nodeTypes[i];
                string path = GetNodeMenuName(type);
                if (string.IsNullOrEmpty(path)) continue;

                if (path.Contains("X Node Extensions") || path.StartsWith("Siphoin Unity Helpers"))
                {
                    string[] parts = path.Split('/');
                    if (parts.Length > 1)
                    {
                        path = "SN Engine/" + string.Join("/", parts.Skip(1));
                    }
                    else
                    {
                        path = "SN Engine/" + path;
                    }
                }

                XNode.Node.DisallowMultipleNodesAttribute disallowAttrib;
                bool disallowed = false;
                if (NodeEditorUtilities.GetAttrib(type, out disallowAttrib))
                {
                    int typeCount = target.nodes.Count(x => x.GetType() == type);
                    if (typeCount >= disallowAttrib.max) disallowed = true;
                }

                if (disallowed)
                    menu.AddDisabledItem(new GUIContent(path));
                else
                    menu.AddItem(new GUIContent(path), false, () =>
                    {
                        XNode.Node node = CreateNode(type, pos);
                        NodeEditorWindow.current.AutoConnect(node);
                    });
            }

            menu.AddSeparator("");
            if (NodeEditorWindow.copyBuffer != null && NodeEditorWindow.copyBuffer.Length > 0)
                menu.AddItem(new GUIContent("Paste"), false, () => NodeEditorWindow.current.PasteNodes(pos));
            else
                menu.AddDisabledItem(new GUIContent("Paste"));

            menu.AddItem(new GUIContent("Preferences"), false, () => NodeEditorReflection.OpenPreferences());
            menu.AddCustomContextMenuItems(target);
        }
    }
}