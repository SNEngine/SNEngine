#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using SNEngine.CharacterSystem;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    public class EmotionSelectorWindow : EditorWindow
    {
        private Action<string> _onSelect;
        private Character _character;
        private string _searchQuery = "";
        private Vector2 _scrollPos;
        private List<Emotion> _filteredEmotions = new List<Emotion>();

        // Virtualization variables
        private const float ROW_HEIGHT = 64f;
        private int _startIndex = 0;
        private int _endIndex = 0;

        public static void Open(Character character, Action<string> onSelect)
        {
            var window = GetWindow<EmotionSelectorWindow>(true, "Emotion Selector", true);
            window._character = character;
            window._onSelect = onSelect;
            window.minSize = new Vector2(300, 450);
            window.ApplyFilter(); // Initialize filter
            window.ShowAuxWindow();
        }

        private void ApplyFilter()
        {
            if (_character == null) return;

            _filteredEmotions = _character.Emotions
                .Where(e => string.IsNullOrEmpty(_searchQuery) || e.Name.IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            // Reset indices for virtualization
            _startIndex = 0;
            _endIndex = Mathf.Min(10, _filteredEmotions.Count); // Start with first 10 items
        }

        private void OnGUI()
        {
            if (_character == null)
            {
                EditorGUILayout.HelpBox("Character is null.", MessageType.Error);
                return;
            }

            DrawHeader();
            DrawSearchBar();
            EditorGUILayout.Space(5);
            DrawEmotionList();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(2);
            EditorGUILayout.LabelField($"Emotions: {_character.name}", EditorStyles.boldLabel);
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

        private void DrawEmotionList()
        {
            if (_filteredEmotions.Count == 0)
            {
                EditorGUILayout.HelpBox("No emotions found.", MessageType.Info);
                return;
            }

            float viewHeight = position.height - 100;
            Rect scrollPositionRect = GUILayoutUtility.GetRect(0, viewHeight, GUILayout.ExpandWidth(true));
            float contentWidth = scrollPositionRect.width - 20;
            Rect viewRect = new Rect(0, 0, contentWidth, _filteredEmotions.Count * ROW_HEIGHT);

            _scrollPos = GUI.BeginScrollView(scrollPositionRect, _scrollPos, viewRect);

            int buffer = 2;
            _startIndex = Mathf.Max(0, Mathf.FloorToInt(_scrollPos.y / ROW_HEIGHT) - buffer);
            _endIndex = Mathf.Min(_filteredEmotions.Count, Mathf.CeilToInt((_scrollPos.y + viewHeight) / ROW_HEIGHT) + buffer);

            GUIStyle pathStyle = new GUIStyle(EditorStyles.miniLabel);
            pathStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
            pathStyle.clipping = TextClipping.Clip;
            pathStyle.wordWrap = false;

            for (int i = _startIndex; i < _endIndex; i++)
            {
                var emotion = _filteredEmotions[i];

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

                Rect iconRect = new Rect(5, (ROW_HEIGHT - 52) / 2, 52, 52);

                Texture preview = null;
                if (emotion.Sprite != null)
                {
                    preview = AssetPreview.GetAssetPreview(emotion.Sprite);
                }

                if (preview == null)
                {
                    var iconContent = EditorGUIUtility.IconContent("d_FilterByLabel");
                    preview = iconContent != null ? iconContent.image : null;
                }

                if (preview != null) GUI.DrawTexture(iconRect, preview, ScaleMode.ScaleToFit);

                // Calculate available text width
                float textWidth = rowRect.width - 130;

                Rect labelRect = new Rect(65, 8, textWidth, 20);
                GUI.Label(labelRect, emotion.Name, EditorStyles.boldLabel);

                // Path with category
                Rect categoryRect = new Rect(65, 26, textWidth, 18);
                GUI.Label(categoryRect, "Character Emotion", pathStyle);

                Rect buttonRect = new Rect(rowRect.width - 70, (ROW_HEIGHT - 28) / 2, 65, 28);
                if (GUI.Button(buttonRect, "Select"))
                {
                    _onSelect?.Invoke(emotion.Name);
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