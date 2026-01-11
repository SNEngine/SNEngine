#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using SNEngine.Graphs;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    public class VariableselectorWindow : EditorWindow
    {
        public enum SelectorMode { All, LocalOnly, GlobalOnly }
        private enum Category { Local, Global }

        private BaseGraph _targetGraph;
        private Action<VariableNode> _onSelect;
        private Type _requiredType;
        private SelectorMode _mode = SelectorMode.All;

        private Category _currentCategory = Category.Local;
        private string _searchQuery = "";
        private Vector2 _scrollPos;

        private List<VariableNode> _localNodes = new List<VariableNode>();
        private List<VariableNode> _globalNodes = new List<VariableNode>();
        private List<VariableNode> _filteredNodes = new List<VariableNode>();

        // Virtualization variables
        private const float ROW_HEIGHT = 48f;
        private int _startIndex = 0;
        private int _endIndex = 0;

        public static void Open(BaseGraph graph, Type requiredType, Action<VariableNode> onSelect, SelectorMode mode = SelectorMode.All)
        {
            var window = GetWindow<VariableselectorWindow>(true, "Variable Selector", true);
            window._targetGraph = graph;
            window._requiredType = requiredType;
            window._onSelect = onSelect;
            window._mode = mode;
            window.minSize = new Vector2(350, 450);
            window.RefreshCache();
            window.ApplyFilter(); // Initialize filter
            window.ShowAuxWindow();
        }

        private void RefreshCache()
        {
            _localNodes.Clear();
            _globalNodes.Clear();

            if (_targetGraph != null)
            {
                _localNodes.AddRange(_targetGraph.nodes.OfType<VariableNode>().Where(IsCompatibleType));
            }

            var globalContainer = Resources.Load<VariableContainerGraph>("VaritableContainerGraph");
            if (globalContainer != null && globalContainer != _targetGraph)
            {
                _globalNodes.AddRange(globalContainer.nodes.OfType<VariableNode>().Where(IsCompatibleType));
            }

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            List<VariableNode> sourceList;

            switch (_mode)
            {
                case SelectorMode.LocalOnly:
                    sourceList = _localNodes;
                    break;
                case SelectorMode.GlobalOnly:
                    sourceList = _globalNodes;
                    break;
                default:
                    sourceList = _currentCategory == Category.Local ? _localNodes : _globalNodes;
                    break;
            }

            _filteredNodes = sourceList
                .Where(n => string.IsNullOrEmpty(_searchQuery) || n.Name.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderBy(n => n.Name)
                .ToList();

            // Reset indices for virtualization
            _startIndex = 0;
            _endIndex = Mathf.Min(10, _filteredNodes.Count); // Start with first 10 items
        }

        private void OnGUI()
        {
            DrawHeader();

            if (_mode == SelectorMode.All)
            {
                DrawCategoryToggles();
            }

            DrawSearchBar();

            EditorGUILayout.Space(5);
            DrawVariableList();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(2);
            EditorGUILayout.LabelField("Variable Selector", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Type: {(_requiredType != null ? _requiredType.Name : "Any")}", EditorStyles.miniLabel);
            GUILayout.Space(2);
            EditorGUILayout.EndVertical();
        }

        private void DrawCategoryToggles()
        {
            GUILayout.Space(2);
            Category newCategory = (Category)GUILayout.Toolbar((int)_currentCategory, new string[] { "Local", "Global" });
            if (newCategory != _currentCategory)
            {
                _currentCategory = newCategory;
                ApplyFilter();
            }
            GUILayout.Space(2);
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
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawVariableList()
        {
            if (_filteredNodes.Count == 0)
            {
                string message = "variables found.";
                if (_mode == SelectorMode.LocalOnly || (_mode == SelectorMode.All && _currentCategory == Category.Local))
                    message = "No local " + message;
                else
                    message = "No global " + message;

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
                VariableNode node = _filteredNodes[i];

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

                // Color strip
                Rect colorStrip = new Rect(0, rowRect.y + 1, 4, rowRect.height - 2);
                EditorGUI.DrawRect(colorStrip, node.Color);

                Rect iconRect = new Rect(10, (ROW_HEIGHT - 28) / 2, 28, 28);

                GUIContent iconContent = EditorGUIUtility.ObjectContent(null, node.GetType());
                Texture nodeIcon = iconContent.image;

                if (nodeIcon == null)
                {
                    nodeIcon = EditorGUIUtility.IconContent("cs Script Icon").image;
                }

                if (nodeIcon != null) GUI.DrawTexture(iconRect, nodeIcon, ScaleMode.ScaleToFit);

                // Calculate available text width
                float textWidth = rowRect.width - 140;

                Rect labelRect = new Rect(45, 6, textWidth, 20);
                GUI.Label(labelRect, node.Name, EditorStyles.boldLabel);

                // Path with category
                bool isGlobal = _mode == SelectorMode.GlobalOnly || (_mode == SelectorMode.All && _currentCategory == Category.Global);
                string categoryText = isGlobal ? $"Container: {node.graph.name}" : node.GetType().Name;
                Rect categoryRect = new Rect(45, 24, textWidth, 18);
                GUI.Label(categoryRect, categoryText, pathStyle);

                Rect buttonRect = new Rect(rowRect.width - 85, (ROW_HEIGHT - 26) / 2, 75, 26);
                if (GUI.Button(buttonRect, "Select"))
                {
                    _onSelect?.Invoke(node);
                    Close();
                }

                GUI.EndGroup();

                Rect lineRect = new Rect(5, (i + 1) * ROW_HEIGHT - 1, rowRect.width - 10, 1);
                EditorGUI.DrawRect(lineRect, new Color(0, 0, 0, 0.1f));
            }

            GUI.EndScrollView();
        }

        private bool IsCompatibleType(VariableNode node)
        {
            if (_requiredType == null) return true;

            Type nodeType = node.GetType();
            while (nodeType != null && nodeType != typeof(object))
            {
                if (nodeType.IsGenericType)
                {
                    Type def = nodeType.GetGenericTypeDefinition();
                    if (def == typeof(VariableNode<>) || def == typeof(VariableCollectionNode<>))
                    {
                        Type nodeVarType = nodeType.GetGenericArguments()[0];
                        return _requiredType.IsAssignableFrom(nodeVarType);
                    }
                }
                nodeType = nodeType.BaseType;
            }
            return false;
        }
    }
}
#endif