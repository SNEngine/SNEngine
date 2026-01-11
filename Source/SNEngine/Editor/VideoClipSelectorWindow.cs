#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace SiphoinUnityHelpers.Editor
{
    public class VideoStreamingSelectorWindow : EditorWindow
    {
        private Action<string> _onSelect;
        private string _searchQuery = "";
        private Vector2 _scrollPos;
        private List<string> _relativePaths = new List<string>();
        private List<string> _filteredRelativePaths = new List<string>();

        private Texture2D _fallbackIcon;
        private const string FALLBACK_ICON_PATH = "Assets/SNEngine/Source/SNEngine/Editor/Sprites/video_editor_icon.png";
        private readonly string[] _extensions = { ".mp4", ".webm", ".mov", ".avi" };

        // Virtualization variables
        private const float ROW_HEIGHT = 48f;
        private int _startIndex = 0;
        private int _endIndex = 0;

        public static void Open(Action<string> onSelect)
        {
            var window = GetWindow<VideoStreamingSelectorWindow>(true, "Video Selector", true);
            window._onSelect = onSelect;
            window.minSize = new Vector2(400, 500);
            window.RefreshCache();
            window.ShowAuxWindow();
        }

        private void OnEnable()
        {
            LoadResources();
        }

        private void LoadResources()
        {
            if (_fallbackIcon == null)
            {
                _fallbackIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(FALLBACK_ICON_PATH);
            }
        }

        private void RefreshCache()
        {
            LoadResources();
            _relativePaths.Clear();

            string fullPath = Application.streamingAssetsPath;
            if (!Directory.Exists(fullPath)) return;

            var files = Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories)
                .Where(f => _extensions.Contains(Path.GetExtension(f).ToLower()));

            foreach (var file in files)
            {
                string rel = file.Replace(fullPath, "").Replace("\\", "/");
                if (rel.StartsWith("/")) rel = rel.Substring(1);
                _relativePaths.Add(rel);
            }

            // Apply current filter
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            _filteredRelativePaths = _relativePaths
                .Where(p => string.IsNullOrEmpty(_searchQuery) || Path.GetFileName(p).IndexOf(_searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            // Reset indices for virtualization
            _startIndex = 0;
            _endIndex = Mathf.Min(10, _filteredRelativePaths.Count); // Start with first 10 items
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawSearchBar();
            EditorGUILayout.Space(5);
            DrawVideoList();
        }

        private void DrawHeader()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(2);
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
            if (GUILayout.Button(EditorGUIUtility.IconContent("Refresh"), GUILayout.Width(25), GUILayout.Height(20)))
            {
                RefreshCache();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawVideoList()
        {
            if (_filteredRelativePaths.Count == 0)
            {
                EditorGUILayout.HelpBox("No video files found in StreamingAssets.", MessageType.Info);
                return;
            }

            float viewHeight = position.height - 100;
            Rect scrollPositionRect = GUILayoutUtility.GetRect(0, viewHeight, GUILayout.ExpandWidth(true));
            float contentWidth = scrollPositionRect.width - 20;
            Rect viewRect = new Rect(0, 0, contentWidth, _filteredRelativePaths.Count * ROW_HEIGHT);

            _scrollPos = GUI.BeginScrollView(scrollPositionRect, _scrollPos, viewRect);

            int buffer = 2;
            _startIndex = Mathf.Max(0, Mathf.FloorToInt(_scrollPos.y / ROW_HEIGHT) - buffer);
            _endIndex = Mathf.Min(_filteredRelativePaths.Count, Mathf.CeilToInt((_scrollPos.y + viewHeight) / ROW_HEIGHT) + buffer);

            GUIStyle pathStyle = new GUIStyle(EditorStyles.miniLabel);
            pathStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
            pathStyle.clipping = TextClipping.Clip;
            pathStyle.wordWrap = false;

            for (int i = _startIndex; i < _endIndex; i++)
            {
                string relPath = _filteredRelativePaths[i];
                string fileName = Path.GetFileName(relPath);

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

                string assetPath = "Assets/StreamingAssets/" + relPath;
                VideoClip clip = AssetDatabase.LoadAssetAtPath<VideoClip>(assetPath);
                Texture icon = null;

                if (clip != null)
                {
                    icon = AssetPreview.GetAssetPreview(clip);
                }

                if (icon == null)
                {
                    icon = _fallbackIcon != null ? _fallbackIcon : EditorGUIUtility.IconContent("VideoClip Icon").image;
                }

                if (icon != null) GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);

                // Calculate available text width
                float textWidth = rowRect.width - 140;

                Rect labelRect = new Rect(45, 6, textWidth, 20);
                GUI.Label(labelRect, fileName, EditorStyles.boldLabel);

                // Path with category
                Rect categoryRect = new Rect(45, 24, textWidth, 18);
                GUI.Label(categoryRect, relPath, pathStyle);

                Rect buttonRect = new Rect(rowRect.width - 85, (ROW_HEIGHT - 26) / 2, 75, 26);
                if (GUI.Button(buttonRect, "Select"))
                {
                    _onSelect?.Invoke(relPath);
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