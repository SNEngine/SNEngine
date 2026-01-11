#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XNode;
using XNodeEditor;

namespace SNEngine.Editor
{
    public class NodeSelectorWindow : EditorWindow
    {
        private Action<Type, Vector2> _onSelect;
        private string _searchQuery = "";
        private Vector2 _scrollPos;
        private List<Type> _allNodeTypes = new List<Type>();
        private List<Type> _filteredNodeTypes = new List<Type>();
        private Vector2 _graphPosition;

        // Virtualization variables
        private const float ROW_HEIGHT = 48f;
        private int _startIndex = 0;
        private int _endIndex = 0;

        public static void Open(Action<Type, Vector2> onSelect, Vector2 graphPosition)
        {
            var window = GetWindow<NodeSelectorWindow>(true, "Node Selector", true);
            window._onSelect = onSelect;
            window._graphPosition = graphPosition;
            window.minSize = new Vector2(350, 450);
            window.RefreshCache();
            window.ShowAuxWindow();
        }

        private void OnEnable()
        {
            // Load any resources if needed
        }

        private void RefreshCache()
        {
            // Get all node types that can be created
            _allNodeTypes = XNodeEditor.NodeEditorReflection.nodeTypes
                .Where(type => !type.IsAbstract && typeof(Node).IsAssignableFrom(type))
                .OrderBy(type => GetNodeMenuName(type))
                .ToList();

            // Apply current filter
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            _filteredNodeTypes = _allNodeTypes
                .Where(t => string.IsNullOrEmpty(_searchQuery) ||
                           GetNodeMenuName(t).IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                           t.Name.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            // Reset indices for virtualization
            _startIndex = 0;
            _endIndex = Mathf.Min(10, _filteredNodeTypes.Count); // Start with first 10 items
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawSearchBar();
            EditorGUILayout.Space(5);
            DrawNodeList();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(2);
            EditorGUILayout.LabelField("Node Selector", EditorStyles.boldLabel);
            GUILayout.Space(2);
            EditorGUILayout.EndVertical();
        }

        private void DrawSearchBar()
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            string newSearchQuery = EditorGUILayout.TextField(new GUIContent("", EditorGUIUtility.FindTexture("Search Icon")), _searchQuery, GUILayout.Height(20));
            if (newSearchQuery != _searchQuery)
            {
                _searchQuery = newSearchQuery;
                ApplyFilter();
            }
            if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
            {
                _searchQuery = "";
                ApplyFilter();
                GUI.FocusControl(null); // Remove focus from search field
            }
            if (GUILayout.Button(EditorGUIUtility.IconContent("Refresh"), GUILayout.Width(25), GUILayout.Height(20)))
            {
                RefreshCache();
            }
            EditorGUILayout.EndHorizontal();
        }

        private string GetNodeMenuName(Type type)
        {
            // Check if type has the CreateNodeMenuAttribute
            XNode.Node.CreateNodeMenuAttribute attrib;
            string name;
            if (XNodeEditor.NodeEditorUtilities.GetAttrib(type, out attrib)) // Return custom path
                name = attrib.menuName;
            else // Return generated path
                name = XNodeEditor.NodeEditorUtilities.NodeDefaultPath(type);

            // Remove HTML color tags from the name
            if (!string.IsNullOrEmpty(name))
            {
                name = System.Text.RegularExpressions.Regex.Replace(name, @"<color=.*?>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                name = System.Text.RegularExpressions.Regex.Replace(name, @"</color>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }

            return name;
        }

        private void DrawNodeList()
        {
            if (_filteredNodeTypes.Count == 0)
            {
                EditorGUILayout.HelpBox("No nodes found matching search.", MessageType.Info);
                return;
            }

            float viewHeight = position.height - 100;
            Rect scrollPositionRect = GUILayoutUtility.GetRect(0, viewHeight, GUILayout.ExpandWidth(true));
            float contentWidth = scrollPositionRect.width - 20;
            Rect viewRect = new Rect(0, 0, contentWidth, _filteredNodeTypes.Count * ROW_HEIGHT);

            _scrollPos = GUI.BeginScrollView(scrollPositionRect, _scrollPos, viewRect);

            int buffer = 2;
            _startIndex = Mathf.Max(0, Mathf.FloorToInt(_scrollPos.y / ROW_HEIGHT) - buffer);
            _endIndex = Mathf.Min(_filteredNodeTypes.Count, Mathf.CeilToInt((_scrollPos.y + viewHeight) / ROW_HEIGHT) + buffer);

            GUIStyle pathStyle = new GUIStyle(EditorStyles.miniLabel);
            pathStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
            pathStyle.clipping = TextClipping.Clip; // ������� ������
            pathStyle.wordWrap = false;

            for (int i = _startIndex; i < _endIndex; i++)
            {
                Type type = _filteredNodeTypes[i];
                string nodeName = GetNodeMenuName(type) ?? type.Name;

                int lastSlash = nodeName.LastIndexOf('/');
                string nodeCategory = lastSlash > 0 ? nodeName.Substring(0, lastSlash) : "General";
                string displayName = lastSlash > 0 ? nodeName.Substring(lastSlash + 1) : nodeName;

                Rect rowRect = new Rect(0, i * ROW_HEIGHT, contentWidth, ROW_HEIGHT);

                // Draw alternating row background
                if (i % 2 == 0)
                {
                    EditorGUI.DrawRect(rowRect, new Color(0.3f, 0.3f, 0.3f, 0.05f));
                }

                if (rowRect.Contains(Event.current.mousePosition))
                {
                    EditorGUI.DrawRect(rowRect, new Color(1f, 1f, 1f, 0.05f));
                    if (Event.current.type == EventType.MouseMove) Repaint();
                }

                GUI.BeginGroup(rowRect);

                Rect iconRect = new Rect(10, (ROW_HEIGHT - 28) / 2, 28, 28);
                GUI.DrawTexture(iconRect, EditorGUIUtility.IconContent("cs Script Icon").image, ScaleMode.ScaleToFit);

                // ������������ ������ ��������� �������, ����� ��� �� �������� �� ������
                float textWidth = rowRect.width - 140;

                // Remove HTML color tags from the display name
                string cleanDisplayName = System.Text.RegularExpressions.Regex.Replace(displayName, @"<color=.*?>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                cleanDisplayName = System.Text.RegularExpressions.Regex.Replace(cleanDisplayName, @"</color>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                Rect labelRect = new Rect(45, 6, textWidth, 20);
                GUI.Label(labelRect, cleanDisplayName, EditorStyles.boldLabel);

                // ��������� ���� � ��������
                Rect categoryRect = new Rect(45, 24, textWidth, 18);
                GUI.Label(categoryRect, nodeCategory, pathStyle);

                Rect buttonRect = new Rect(rowRect.width - 85, (ROW_HEIGHT - 26) / 2, 75, 26);
                if (GUI.Button(buttonRect, "Create"))
                {
                    _onSelect?.Invoke(type, _graphPosition);
                    Close();
                }

                GUI.EndGroup();

                Rect lineRect = new Rect(5, (i + 1) * ROW_HEIGHT - 1, rowRect.width - 10, 1);
                EditorGUI.DrawRect(lineRect, new Color(0, 0, 0, 0.1f));
            }

            GUI.EndScrollView();
        }

    }
}
#endif