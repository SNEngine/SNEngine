#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using SNEngine.CharacterSystem;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    public class CharacterSelectorWindow : EditorWindow
    {
        private Action<Character> _onSelect;
        private string _searchQuery = "";
        private Vector2 _scrollPos;
        private List<Character> _characters = new List<Character>();
        private List<Character> _filteredCharacters = new List<Character>();

        // Virtualization variables
        private const float ROW_HEIGHT = 48f;
        private int _startIndex = 0;
        private int _endIndex = 0;

        public static void Open(Action<Character> onSelect)
        {
            var window = GetWindow<CharacterSelectorWindow>(true, "Character Selector", true);
            window._onSelect = onSelect;
            window.minSize = new Vector2(350, 450);
            window.RefreshCache();
            window.ShowAuxWindow();
        }

        private void RefreshCache()
        {
            _characters = Resources.LoadAll<Character>("").OrderBy(c => c.name).ToList();

            // Apply current filter
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            _filteredCharacters = _characters
                .Where(c => string.IsNullOrEmpty(_searchQuery) || c.name.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            // Reset indices for virtualization
            _startIndex = 0;
            _endIndex = Mathf.Min(10, _filteredCharacters.Count); // Start with first 10 items
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawSearchBar();
            EditorGUILayout.Space(5);
            DrawCharacterList();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(2);
            EditorGUILayout.LabelField("Character Selector", EditorStyles.boldLabel);
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
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawCharacterList()
        {
            if (_filteredCharacters.Count == 0)
            {
                EditorGUILayout.HelpBox("No characters found in Resources.", MessageType.Info);
                return;
            }

            float viewHeight = position.height - 100;
            Rect scrollPositionRect = GUILayoutUtility.GetRect(0, viewHeight, GUILayout.ExpandWidth(true));
            float contentWidth = scrollPositionRect.width - 20;
            Rect viewRect = new Rect(0, 0, contentWidth, _filteredCharacters.Count * ROW_HEIGHT);

            _scrollPos = GUI.BeginScrollView(scrollPositionRect, _scrollPos, viewRect);

            int buffer = 2;
            _startIndex = Mathf.Max(0, Mathf.FloorToInt(_scrollPos.y / ROW_HEIGHT) - buffer);
            _endIndex = Mathf.Min(_filteredCharacters.Count, Mathf.CeilToInt((_scrollPos.y + viewHeight) / ROW_HEIGHT) + buffer);

            GUIStyle pathStyle = new GUIStyle(EditorStyles.miniLabel);
            pathStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
            pathStyle.clipping = TextClipping.Clip;
            pathStyle.wordWrap = false;

            for (int i = _startIndex; i < _endIndex; i++)
            {
                Character character = _filteredCharacters[i];

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

                Texture icon = AssetPreview.GetAssetPreview(character);
                if (icon == null)
                {
                    icon = EditorGUIUtility.ObjectContent(character, typeof(Character)).image;
                }

                if (icon != null) GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);

                // Calculate available text width
                float textWidth = rowRect.width - 140;

                Rect labelRect = new Rect(45, 6, textWidth, 20);
                GUI.Label(labelRect, character.name, EditorStyles.boldLabel);

                // Path with category
                Rect categoryRect = new Rect(45, 24, textWidth, 18);
                GUI.Label(categoryRect, "Character Asset", pathStyle);

                Rect buttonRect = new Rect(rowRect.width - 85, (ROW_HEIGHT - 26) / 2, 75, 26);
                if (GUI.Button(buttonRect, "Select"))
                {
                    _onSelect?.Invoke(character);
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