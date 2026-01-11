using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using SNEngine.Graphs;
using SiphoinUnityHelpers.XNodeExtensions.Editor;
using SiphoinUnityHelpers.XNodeExtensions;

namespace SNEngine.Editor
{
    [CustomNodeGraphEditor(typeof(VariableContainerGraph))]
    public class VariableContainerGraphEditor : FilteredNodeGraphEditor
    {
        private static readonly Dictionary<Type, bool> _typeCache = new Dictionary<Type, bool>();

        private bool _isSidebarVisible = true;
        private Vector2 _sidebarScroll;
        private const float SidebarWidth = 220f;

        protected override bool IsNodeTypeAllowed(Type nodeType)
        {
            if (_typeCache.TryGetValue(nodeType, out bool allowed))
                return allowed;

            allowed = CheckIfAllowed(nodeType);
            _typeCache[nodeType] = allowed;
            return allowed;
        }

        private bool CheckIfAllowed(Type nodeType)
        {
            if (typeof(VariableNode).IsAssignableFrom(nodeType))
                return true;

            if (typeof(SummaryNode).IsAssignableFrom(nodeType))
                return true;
            if (typeof(GroupNode).IsAssignableFrom(nodeType))
                return true;

            Type currentType = nodeType;
            while (currentType != null && currentType != typeof(object))
            {
                if (currentType.IsGenericType &&
                    currentType.GetGenericTypeDefinition().Name.Contains("SetVaritableNode"))
                {
                    return true;
                }
                currentType = currentType.BaseType;
            }

            return nodeType.Name.Contains("SetVaritable");
        }

        public override void OnGUI()
        {
            // Draw toolbar buttons at the top
            DrawSeparator();

            NodeEditorWindow window = NodeEditorWindow.current;
            if (window == null) return;

            float windowWidth = window.position.width;
            float windowHeight = window.position.height;
            float currentWidth = _isSidebarVisible ? SidebarWidth : 25;

            Rect sidebarRect = new Rect(windowWidth - currentWidth, 0, currentWidth, windowHeight);

            GUI.Box(sidebarRect, "", (GUIStyle)"hostview");

            Rect toggleBtnRect = new Rect(sidebarRect.x + 5, 5, 15, 20);
            if (GUI.Button(toggleBtnRect, _isSidebarVisible ? ">" : "<", EditorStyles.miniButton))
            {
                _isSidebarVisible = !_isSidebarVisible;
            }

            if (_isSidebarVisible)
            {
                Rect areaRect = new Rect(sidebarRect.x, 0, SidebarWidth, windowHeight);
                GUILayout.BeginArea(areaRect);

                GUILayout.Space(10);
                EditorGUILayout.LabelField("VARIABLES", EditorStyles.centeredGreyMiniLabel);
                GUILayout.Space(10);

                _sidebarScroll = EditorGUILayout.BeginScrollView(_sidebarScroll, GUIStyle.none, GUI.skin.verticalScrollbar);

                var variables = target.nodes.OfType<VariableNode>()
                    .OrderBy(x => x.Name)
                    .ToList();

                for (int i = 0; i < variables.Count; i++)
                {
                    DrawVariableRow(window, variables[i], i);
                }

                EditorGUILayout.EndScrollView();
                GUILayout.EndArea();
            }
        }

        private void DrawSeparator()
        {
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawVariableRow(NodeEditorWindow window, VariableNode node, int index)
        {
            float rowHeight = 36f;
            Rect rowRect = EditorGUILayout.GetControlRect(false, rowHeight);

            // ����������� ����� ��������
            if (index % 2 != 0)
            {
                EditorGUI.DrawRect(rowRect, new Color(0, 0, 0, 0.15f));
            }

            // ��������� ��� ���������
            if (rowRect.Contains(Event.current.mousePosition))
            {
                EditorGUI.DrawRect(rowRect, new Color(1, 1, 1, 0.08f));
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    FocusNode(window, node);
                    Event.current.Use();
                }
            }

            // ������� ������� (������ ����)
            Rect colorStrip = new Rect(rowRect.x + 2, rowRect.y + 4, 3, rowRect.height - 8);
            EditorGUI.DrawRect(colorStrip, node.Color);

            // ������
            GUIContent iconContent = EditorGUIUtility.ObjectContent(null, node.GetType());
            Texture nodeIcon = iconContent.image;
            if (nodeIcon == null) nodeIcon = EditorGUIUtility.IconContent("cs Script Icon").image;

            Rect iconRect = new Rect(rowRect.x + 12, rowRect.y + (rowHeight - 24) / 2, 24, 24);
            if (nodeIcon != null) GUI.DrawTexture(iconRect, nodeIcon, ScaleMode.ScaleToFit);

            // �����
            Rect labelRect = new Rect(iconRect.xMax + 8, rowRect.y, rowRect.width - iconRect.xMax - 15, rowHeight);
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.alignment = TextAnchor.MiddleLeft;
            labelStyle.fontSize = 11;

            GUI.Label(labelRect, node.Name, labelStyle);

            GUILayout.Space(1);
        }

        private void FocusNode(NodeEditorWindow window, XNode.Node node)
        {
            window.panOffset = -node.position;
            Selection.activeObject = node;
            window.Repaint();
        }
    }
}