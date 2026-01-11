using UnityEditor;
using UnityEngine;
using System.IO;

namespace SNEngine.Editor
{
    public abstract class BaseRenameWindow<T> : EditorWindow where T : UnityEngine.Object
    {
        protected T targetAsset;
        protected string newName;
        private bool _isFirstFrame = true;
        protected string headerSubtitle = "Rename Asset";

        protected virtual void OnGUI()
        {
            if (targetAsset == null)
            {
                Close();
                return;
            }

            DrawVisualHeader();
            GUILayout.Space(15);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("New Name", EditorStyles.miniBoldLabel);
            newName = EditorGUILayout.TextField(newName, GUILayout.Height(24));

            if (_isFirstFrame && Event.current.type == EventType.Repaint)
            {
                _isFirstFrame = false;
            }

            if (GUI.GetNameOfFocusedControl() == "RenameTextField")
            {
                TextEditor te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                if (te != null && te.text == newName && Event.current.type == EventType.Layout)
                {
                    te.SelectAll();
                }
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(20);
            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            DrawButtons();
            GUILayout.Space(12);

            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Return)
                {
                    ApplyAction();
                    Event.current.Use();
                    GUIUtility.ExitGUI();
                }
                else if (Event.current.keyCode == KeyCode.Escape)
                {
                    this.Close();
                    Event.current.Use();
                    GUIUtility.ExitGUI();
                }
            }
        }

        protected virtual void DrawVisualHeader()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(8);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);

            string path = AssetDatabase.GetAssetPath(targetAsset);
            Texture icon = AssetDatabase.GetCachedIcon(path);

            if (icon != null)
            {
                Rect iconRect = GUILayoutUtility.GetRect(32, 32, GUILayout.Width(32), GUILayout.Height(32));
                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
            }

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical();
            GUIStyle subTitleStyle = new GUIStyle(EditorStyles.miniLabel);
            subTitleStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
            EditorGUILayout.LabelField(headerSubtitle, subTitleStyle);

            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
            titleStyle.fontSize = 12;
            EditorGUILayout.LabelField(targetAsset.name, titleStyle);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(8);
            EditorGUILayout.EndVertical();
        }

        protected virtual void DrawButtons()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel", GUILayout.Width(90), GUILayout.Height(24)))
            {
                this.Close();
                GUIUtility.ExitGUI();
            }

            GUI.backgroundColor = new Color(0.2f, 0.45f, 0.7f);
            if (GUILayout.Button("Rename", GUILayout.Width(90), GUILayout.Height(24)))
            {
                ApplyAction();
                GUIUtility.ExitGUI();
            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(15);
            EditorGUILayout.EndHorizontal();
        }

        protected virtual void ApplyAction()
        {
            if (string.IsNullOrWhiteSpace(newName) || newName == targetAsset.name)
            {
                this.Close();
                return;
            }

            string assetPath = AssetDatabase.GetAssetPath(targetAsset);
            string directory = Path.GetDirectoryName(assetPath);
            string extension = Path.GetExtension(assetPath);
            string newPath = Path.Combine(directory, newName + extension);

            string error = AssetDatabase.ValidateMoveAsset(assetPath, newPath);
            if (!string.IsNullOrEmpty(error))
            {
                EditorUtility.DisplayDialog("Error", $"Cannot rename: {error}", "OK");
                return;
            }

            AssetDatabase.RenameAsset(assetPath, newName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            this.Close();
        }
    }
}