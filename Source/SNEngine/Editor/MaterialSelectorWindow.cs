#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SiphoinUnityHelpers.Editor
{
    public class MaterialSelectorWindow : EditorWindow
    {
        private Action<Material> _onSelect;
        private string _searchQuery = "";
        private Vector2 _scrollPos;

        private struct MaterialInfo
        {
            public string Path;
            public string Name;
            public string ShaderName;
        }

        private List<MaterialInfo> _allAssets = new List<MaterialInfo>();
        private List<MaterialInfo> _filteredAssets = new List<MaterialInfo>();

        private float _iconSize = 90f;
        private const float SPACING = 10f;
        private GUIStyle _nameLabelStyle;

        public static void Open(Action<Material> onSelect)
        {
            var window = GetWindow<MaterialSelectorWindow>(true, "Material Selector", true);
            window._onSelect = onSelect;
            window.RefreshCache();
            window.ShowAuxWindow();
        }

        private void RefreshCache()
        {
            string[] guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
            _allAssets.Clear();

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("/Editor/") || path.Contains("Assets/Editor/")) continue;

                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat == null) continue;

                _allAssets.Add(new MaterialInfo
                {
                    Path = path,
                    Name = mat.name,
                    ShaderName = mat.shader != null ? mat.shader.name : "Missing Shader"
                });
            }
            UpdateFilter();
        }

        private void UpdateFilter()
        {
            var query = _allAssets.AsEnumerable();
            if (!string.IsNullOrEmpty(_searchQuery))
            {
                query = query.Where(s => s.Name.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase) || s.ShaderName.Contains(_searchQuery, StringComparison.OrdinalIgnoreCase));
            }
            _filteredAssets = query.ToList();
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

            float viewWidth = position.width - 15;
            int columns = Mathf.Max(1, Mathf.FloorToInt(viewWidth / (_iconSize + SPACING)));
            float actualSpacing = (viewWidth - (columns * _iconSize)) / (columns + 1);
            int totalRows = Mathf.CeilToInt((float)_filteredAssets.Count / columns);
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
                    if (index >= _filteredAssets.Count) break;

                    float x = actualSpacing + col * (_iconSize + actualSpacing);
                    float y = row * rowHeight + (SPACING / 2);
                    Rect itemRect = new Rect(x, y, _iconSize, _iconSize);

                    DrawMaterialItem(itemRect, _filteredAssets[index]);
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
        }

        private void DrawMaterialItem(Rect rect, MaterialInfo info)
        {
            if (GUI.Button(rect, new GUIContent("", $"{info.Name}\nShader: {info.ShaderName}")))
            {
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(info.Path);
                _onSelect?.Invoke(mat);
                Close();
            }

            Texture2D preview = AssetPreview.GetAssetPreview(AssetDatabase.LoadMainAssetAtPath(info.Path));
            if (preview != null) GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit);

            Rect labelBgRect = new Rect(rect.x, rect.yMax - 18, rect.width, 18);
            EditorGUI.DrawRect(labelBgRect, new Color(0, 0, 0, 0.7f));
            GUI.Label(labelBgRect, info.Name, _nameLabelStyle);
        }
    }
}
#endif