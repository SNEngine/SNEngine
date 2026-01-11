#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SNEngine.Editor
{
    public class SpriteSelectorWindow : EditorWindow
    {
        public enum SpriteCategory { All, Backgrounds }

        private Action<Sprite> _onSelect;
        private string _searchQuery = "";
        private Vector2 _scrollPos;
        private SpriteCategory _currentCategory = SpriteCategory.All;

        private struct SpriteInfo
        {
            public string Path;
            public string Name;
            public int Width;
            public int Height;
        }

        private List<SpriteInfo> _allSprites = new List<SpriteInfo>();
        private List<SpriteInfo> _filteredSprites = new List<SpriteInfo>();

        private float _iconSize = 90f;
        private const float SPACING = 10f;
        private GUIStyle _nameLabelStyle;

        public static void Open(Action<Sprite> onSelect, SpriteCategory initialCategory = SpriteCategory.All)
        {
            var window = GetWindow<SpriteSelectorWindow>(true, "Sprite Selector", true);
            window._onSelect = onSelect;
            window._currentCategory = initialCategory;
            window.RefreshCache();
            window.ShowAuxWindow();
        }

        private void RefreshCache()
        {
            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets" });
            _allSprites.Clear();

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("/Editor/") || path.Contains("Assets/Editor/")) continue;

                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                int w = 0, h = 0;

                if (importer != null)
                {
                    object[] args = new object[] { 0, 0 };
                    var method = typeof(TextureImporter).GetMethod("GetWidthAndHeight", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    method?.Invoke(importer, args);
                    w = (int)args[0];
                    h = (int)args[1];
                }

                _allSprites.Add(new SpriteInfo { Path = path, Name = System.IO.Path.GetFileNameWithoutExtension(path), Width = w, Height = h });
            }
            UpdateFilter();
        }

        private void UpdateFilter()
        {
            var query = _allSprites.AsEnumerable();
            if (_currentCategory == SpriteCategory.Backgrounds)
            {
                query = query.Where(s => (s.Width >= 1280 && s.Height >= 720) || s.Name.Contains("bg", StringComparison.OrdinalIgnoreCase) || s.Path.Contains("/Backgrounds/"));
            }
            if (!string.IsNullOrEmpty(_searchQuery))
            {
                query = query.Where(s => s.Name.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase));
            }
            _filteredSprites = query.ToList();
        }

        private void OnGUI()
        {
            if (_nameLabelStyle == null)
            {
                _nameLabelStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.white },
                    fontSize = 10,
                    clipping = TextClipping.Ellipsis
                };
            }

            DrawTopPanel();

            // Расчет сетки
            float viewWidth = position.width - 15; // Запас под скроллбар
            int columns = Mathf.Max(1, Mathf.FloorToInt(viewWidth / (_iconSize + SPACING)));
            float actualSpacing = (viewWidth - (columns * _iconSize)) / (columns + 1);
            int totalRows = Mathf.CeilToInt((float)_filteredSprites.Count / columns);
            float rowHeight = _iconSize + SPACING;

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            Rect contentRect = GUILayoutUtility.GetRect(viewWidth, totalRows * rowHeight);

            int startRow = Mathf.FloorToInt(_scrollPos.y / rowHeight);
            int endRow = Mathf.Min(totalRows, startRow + Mathf.CeilToInt(position.height / rowHeight) + 1);

            for (int row = startRow; row < endRow; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    int index = row * columns + col;
                    if (index >= _filteredSprites.Count) break;

                    float x = actualSpacing + col * (_iconSize + actualSpacing);
                    float y = row * rowHeight + (SPACING / 2);
                    Rect itemRect = new Rect(x, y, _iconSize, _iconSize);

                    DrawSpriteItem(itemRect, _filteredSprites[index]);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawTopPanel()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUI.BeginChangeCheck();
            _searchQuery = EditorGUILayout.TextField(_searchQuery, EditorStyles.toolbarSearchField);
            if (EditorGUI.EndChangeCheck()) UpdateFilter();
            _iconSize = GUILayout.HorizontalSlider(_iconSize, 60f, 300f, GUILayout.Width(100));
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60))) RefreshCache();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUI.BeginChangeCheck();
            _currentCategory = (SpriteCategory)GUILayout.SelectionGrid((int)_currentCategory, new string[] { "All Assets", "Backgrounds" }, 2, EditorStyles.miniButton);
            if (EditorGUI.EndChangeCheck()) UpdateFilter();
            GUILayout.EndHorizontal();
        }

        private void DrawSpriteItem(Rect rect, SpriteInfo info)
        {
            if (GUI.Button(rect, new GUIContent("", $"{info.Name}\n{info.Width}x{info.Height}")))
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(info.Path);
                _onSelect?.Invoke(sprite);
                Close();
            }

            Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(info.Path);
            if (s != null)
            {
                Texture2D preview = AssetPreview.GetAssetPreview(s);
                if (preview != null) GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit);
            }

            Rect labelBgRect = new Rect(rect.x, rect.yMax - 18, rect.width, 18);
            EditorGUI.DrawRect(labelBgRect, new Color(0, 0, 0, 0.7f));
            GUI.Label(labelBgRect, info.Name, _nameLabelStyle);
        }
    }
}
#endif