using UnityEngine;
using UnityEditor;
using XNodeEditor;
using SiphoinUnityHelpers.XNodeExtensions;

namespace SiphoinUnityHelpers.XNodeExtensions.Editor
{
    [CustomNodeEditor(typeof(ExitNode))]
    public class ExitNodeEditor : NodeEditor
    {
        private Texture2D _icon;
        private const string IconPath = "Assets/SNEngine/Source/SNEngine/Editor/Sprites/node_exit_icon_editor.png";

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

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            if (enterPort != null)
            {
                NodeEditorGUILayout.PortField(GUIContent.none, enterPort, GUILayout.Width(20));
            }
            else
            {
                GUILayout.Space(20);
            }

            GUILayout.FlexibleSpace();

            if (_icon != null)
            {
                Rect iconRect = GUILayoutUtility.GetRect(64, 64);
                GUI.DrawTexture(iconRect, _icon, ScaleMode.ScaleToFit);
            }

            GUILayout.FlexibleSpace();

            GUILayout.Space(20);

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