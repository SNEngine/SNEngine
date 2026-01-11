#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;
using XNodeEditor;
using SiphoinUnityHelpers.Editor;
using SNEngine.VideoPlayerSystem;
using System.IO;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(PlayVideoNode))]
    public class PlayVideoNodeEditor : NodeEditor
    {
        private Texture2D _fallbackIcon;
        private const string FALLBACK_ICON_PATH = "Assets/SNEngine/Source/SNEngine/Editor/Sprites/video_editor_icon.png";

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            if (_fallbackIcon == null)
            {
                _fallbackIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(FALLBACK_ICON_PATH);
            }

            foreach (var field in NodeEditorGUILayout.GetFilteredFields(serializedObject))
            {
                if (field.name == "_videoPath") continue;
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(field.name));
            }

            GUILayout.Space(5);

            SerializedProperty pathProp = serializedObject.FindProperty("_videoPath");
            string currentPath = pathProp.stringValue;
            bool hasPath = !string.IsNullOrEmpty(currentPath);

            Color prevBg = GUI.backgroundColor;
            GUI.backgroundColor = hasPath ? new Color(0.4f, 0.75f, 0.45f) : new Color(0.75f, 0.4f, 0.4f);

            EditorGUILayout.BeginVertical(GUI.skin.box);

            float boxHeight = hasPath ? 70 : 30;
            Rect rect = GUILayoutUtility.GetRect(10, boxHeight);

            if (GUI.Button(rect, hasPath ? "" : "Select Video Path"))
            {
                VideoStreamingSelectorWindow.Open((selectedPath) =>
                {
                    var so = new SerializedObject(target);
                    var p = so.FindProperty("_videoPath");
                    if (p != null)
                    {
                        p.stringValue = selectedPath;
                        so.ApplyModifiedProperties();
                    }
                });
            }

            if (hasPath)
            {
                string fullAssetPath = Path.Combine("Assets/StreamingAssets", currentPath).Replace("\\", "/");
                VideoClip clip = AssetDatabase.LoadAssetAtPath<VideoClip>(fullAssetPath);
                Texture preview = null;

                if (clip != null)
                {
                    preview = AssetPreview.GetAssetPreview(clip);
                }

                if (preview == null || (clip != null && AssetPreview.IsLoadingAssetPreview(clip.GetInstanceID())))
                {
                    preview = _fallbackIcon != null ? _fallbackIcon : EditorGUIUtility.IconContent("VideoClip Icon").image;
                }

                if (preview != null)
                {
                    GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit);
                }

                Rect labelRect = new Rect(rect.x, rect.yMax - 16, rect.width, 16);
                EditorGUI.DrawRect(labelRect, new Color(0, 0, 0, 0.6f));
                GUI.Label(labelRect, Path.GetFileName(currentPath), EditorStyles.centeredGreyMiniLabel);
            }

            EditorGUILayout.EndVertical();

            GUI.backgroundColor = prevBg;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif