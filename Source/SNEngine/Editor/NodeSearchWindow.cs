#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XNode;
using XNodeEditor;
using SiphoinUnityHelpers.XNodeExtensions;

namespace SNEngine.Editor
{
    public class NodeSearchWindow : EditorWindow
    {
        private BaseGraph _targetGraph;
        private string _searchQuery = "";
        private Vector2 _scrollPos;
        private List<BaseNode> _allNodes = new List<BaseNode>();
        private List<BaseNode> _filteredNodes = new List<BaseNode>();

        // Cached icon texture for all nodes
        private Texture2D _nodeIconTexture;

        // Virtualization variables
        private const float ROW_HEIGHT = 48f;
        private int _startIndex = 0;
        private int _endIndex = 0;

        public static void Open(BaseGraph targetGraph)
        {
            var window = GetWindow<NodeSearchWindow>(true, "Node Search", true);
            window._targetGraph = targetGraph;
            window.LoadNodeIcon();
            window.minSize = new Vector2(400, 500);
            window.RefreshCache();
            window.ApplyFilter();
            window.ShowAuxWindow();
        }

        private void RefreshCache()
        {
            if (_targetGraph != null)
            {
                _allNodes = _targetGraph.nodes.OfType<BaseNode>().ToList();
            }
            else
            {
                _allNodes.Clear();
            }
        }

        private void ApplyFilter()
        {
            if (_allNodes == null)
            {
                _filteredNodes = new List<BaseNode>();
                return;
            }

            _filteredNodes = _allNodes
                .Where(n =>
                {
                    string nodeName = n.name ?? "";
                    string nodeGuid = n.GUID ?? "";

                    // Remove HTML color tags from node name for search comparison
                    string cleanNodeName = System.Text.RegularExpressions.Regex.Replace(nodeName, @"<color=.*?>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    cleanNodeName = System.Text.RegularExpressions.Regex.Replace(cleanNodeName, @"</color>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                    return string.IsNullOrEmpty(_searchQuery) ||
                           cleanNodeName.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                           n.GetType().Name.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                           nodeGuid.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0;
                })
                .ToList();

            // Reset indices for virtualization
            _startIndex = 0;
            _endIndex = Mathf.Min(10, _filteredNodes.Count); // Start with first 10 items
        }

        private void LoadNodeIcon()
        {
            // Try loading from Assets path
            _nodeIconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/SNEngine/Source/SNEngine/Editor/Sprites/node_editor_icon.png");
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
            EditorGUILayout.LabelField("Node Search", EditorStyles.boldLabel);
            if (_targetGraph != null)
            {
                EditorGUILayout.LabelField($"Graph: {_targetGraph.name}", EditorStyles.miniLabel);
            }
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
                ApplyFilter();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawNodeList()
        {
            if (_filteredNodes.Count == 0)
            {
                string message = "No nodes found.";
                if (!string.IsNullOrEmpty(_searchQuery))
                {
                    message = "No nodes match the search query.";
                }
                EditorGUILayout.HelpBox(message, MessageType.Info);
                return;
            }

            float viewHeight = position.height - 100;
            Rect scrollPositionRect = GUILayoutUtility.GetRect(0, viewHeight, GUILayout.ExpandWidth(true));
            float contentWidth = scrollPositionRect.width - 20;
            Rect viewRect = new Rect(0, 0, contentWidth, _filteredNodes.Count * ROW_HEIGHT);

            _scrollPos = GUI.BeginScrollView(scrollPositionRect, _scrollPos, viewRect);

            int buffer = 2;
            _startIndex = Mathf.Max(0, Mathf.FloorToInt(_scrollPos.y / ROW_HEIGHT) - buffer);
            _endIndex = Mathf.Min(_filteredNodes.Count, Mathf.CeilToInt((_scrollPos.y + viewHeight) / ROW_HEIGHT) + buffer);

            GUIStyle pathStyle = new GUIStyle(EditorStyles.miniLabel);
            pathStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
            pathStyle.clipping = TextClipping.Clip;
            pathStyle.wordWrap = false;

            for (int i = _startIndex; i < _endIndex; i++)
            {
                BaseNode node = _filteredNodes[i];

                Rect rowRect = new Rect(0, i * ROW_HEIGHT, contentWidth, ROW_HEIGHT);

                if (rowRect.Contains(Event.current.mousePosition))
                {
                    EditorGUI.DrawRect(rowRect, new Color(1f, 1f, 1f, 0.05f));
                    if (Event.current.type == EventType.MouseMove) Repaint();
                }

                // Draw alternating row background
                if (i % 2 == 0)
                {
                    EditorGUI.DrawRect(rowRect, new Color(0.3f, 0.3f, 0.3f, 0.05f));
                }

                GUI.BeginGroup(rowRect);

                Rect iconRect = new Rect(10, (ROW_HEIGHT - 28) / 2, 28, 28);

                // Use the specific node editor icon for all nodes
                if (_nodeIconTexture != null)
                {
                    GUI.DrawTexture(iconRect, _nodeIconTexture, ScaleMode.ScaleToFit);
                }
                else
                {
                    // Fallback to generic script icon if the custom icon is not loaded
                    Texture fallbackIcon = EditorGUIUtility.IconContent("cs Script Icon").image;
                    if (fallbackIcon != null) GUI.DrawTexture(iconRect, fallbackIcon, ScaleMode.ScaleToFit);
                }

                // Calculate available text width
                float textWidth = rowRect.width - 140;

                string displayName = string.IsNullOrEmpty(node.name) ? ObjectNames.NicifyVariableName(node.GetType().Name) : node.name;
                // Remove HTML color tags from the display name
                displayName = System.Text.RegularExpressions.Regex.Replace(displayName, @"<color=.*?>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                displayName = System.Text.RegularExpressions.Regex.Replace(displayName, @"</color>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                Rect labelRect = new Rect(45, 6, textWidth, 20);
                GUI.Label(labelRect, displayName, EditorStyles.boldLabel);

                // Display GUID instead of type
                string nodeGuid = node.GUID ?? "No GUID";
                Rect guidRect = new Rect(45, 24, textWidth, 18);
                GUI.Label(guidRect, nodeGuid, pathStyle);

                Rect buttonRect = new Rect(rowRect.width - 85, (ROW_HEIGHT - 26) / 2, 75, 26);
                if (GUI.Button(buttonRect, "Focus"))
                {
                    FocusOnNode(node);
                    Close();
                }

                GUI.EndGroup();

                Rect lineRect = new Rect(5, (i + 1) * ROW_HEIGHT - 1, rowRect.width - 10, 1);
                EditorGUI.DrawRect(lineRect, new Color(0, 0, 0, 0.1f));
            }

            GUI.EndScrollView();
        }

        private void FocusOnNode(BaseNode node)
        {
            // Find the NodeEditorWindow that's currently editing this graph
            NodeEditorWindow window = NodeEditorWindow.Open(_targetGraph);
            if (window != null)
            {
                // Center the view on the node
                Vector2 nodeSize = window.nodeSizes.ContainsKey(node) ? window.nodeSizes[node] / 2 : new Vector2(100, 50);
                window.panOffset = -node.position - nodeSize;
                
                // Select the node
                window.SelectNode(node, false);
            }
        }
    }
}
#endif