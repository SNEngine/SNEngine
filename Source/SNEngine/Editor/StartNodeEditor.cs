using UnityEngine;
using UnityEditor;
using XNodeEditor;
using SiphoinUnityHelpers.XNodeExtensions;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    [CustomNodeEditor(typeof(StartNode))]
    public class StartNodeEditor : NodeEditor
    {
        private Texture2D _icon;
        private const string IconPath = "Assets/SNEngine/Source/SNEngine/Editor/Sprites/node_start_icon_editor.png";

        public override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            if (_icon == null)
            {
                _icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath);
            }

            XNode.NodePort enterPort = target.GetPort("_enter");
            XNode.NodePort exitPort = target.GetPort("_exit");

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            if (enterPort != null)
            {
                NodeEditorGUILayout.PortField(GUIContent.none, enterPort, GUILayout.Width(0));
            }

            GUILayout.FlexibleSpace();

            if (_icon != null)
            {
                Rect iconRect = GUILayoutUtility.GetRect(64, 64);
                GUI.DrawTexture(iconRect, _icon, ScaleMode.ScaleToFit);
            }

            GUILayout.FlexibleSpace();

            if (exitPort != null)
            {
                NodeEditorGUILayout.PortField(GUIContent.none, exitPort, GUILayout.Width(0));
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 210;
        }
    }
}