#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using SNEngine.Graphs;
using SiphoinUnityHelpers.XNodeExtensions.Variables.DictionarySystem;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    public class DictionarySelectorWindow : EditorWindow
    {
        public enum SelectorMode { LocalOnly, GlobalOnly }

        private BaseGraph _targetGraph;
        private Action<VariableNode> _onSelect;
        private Type _keyType;
        private Type _valueType;
        private SelectorMode _mode;

        private string _searchQuery = "";
        private Vector2 _scrollPos;
        private List<VariableNode> _nodes = new List<VariableNode>();

        public static void Open(BaseGraph graph, Type keyType, Type valueType, Action<VariableNode> onSelect, SelectorMode mode)
        {
            var window = GetWindow<DictionarySelectorWindow>(true, "Dictionary Selector", true);
            window._targetGraph = graph;
            window._keyType = keyType;
            window._valueType = valueType;
            window._onSelect = onSelect;
            window._mode = mode;
            window.RefreshCache();
            window.ShowAuxWindow();
        }

        private void RefreshCache()
        {
            _nodes.Clear();
            var allNodes = new List<VariableNode>();

            if (_mode == SelectorMode.LocalOnly)
            {
                if (_targetGraph != null)
                {
                    allNodes.AddRange(_targetGraph.nodes.OfType<VariableNode>());
                }
            }
            else if (_mode == SelectorMode.GlobalOnly)
            {
                var containers = Resources.LoadAll<VariableContainerGraph>("");
                foreach (var container in containers)
                {
                    allNodes.AddRange(container.nodes.OfType<VariableNode>());
                }
            }

            foreach (var node in allNodes)
            {
                if (IsCompatibleType(node)) _nodes.Add(node);
            }
        }

        private bool IsCompatibleType(VariableNode node)
        {
            Type type = node.GetType();
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.Name.StartsWith("DictionaryVariableNode"))
                {
                    Type[] args = type.GetGenericArguments();
                    if (args.Length >= 2)
                    {
                        return _keyType.IsAssignableFrom(args[0]) && _valueType.IsAssignableFrom(args[1]);
                    }
                }
                type = type.BaseType;
            }
            return false;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField($"Mode: {_mode}", EditorStyles.miniLabel);
            _searchQuery = EditorGUILayout.TextField("Search", _searchQuery);
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            foreach (var node in _nodes)
            {
                if (!string.IsNullOrEmpty(_searchQuery) && !node.Name.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase)) continue;

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                var rect = GUILayoutUtility.GetRect(4, 40, GUILayout.Width(4));
                EditorGUI.DrawRect(rect, node.Color);

                EditorGUILayout.LabelField(node.Name, EditorStyles.boldLabel);
                if (GUILayout.Button("Select", GUILayout.Width(70)))
                {
                    _onSelect?.Invoke(node);
                    Close();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
#endif