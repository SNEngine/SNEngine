#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using SiphoinUnityHelpers.XNodeExtensions.NodesControlExecutes;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(GroupCallsNode))]
    public class GroupCallsNodeEditor : NodeEditor
    {
        private GUIStyle _headerStyle;
        private GUIStyle _bodyStyle;

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            DrawHeader();

            GUILayout.Space(5);

            XNode.NodePort enterPort = target.GetPort("_enter");
            XNode.NodePort exitPort = target.GetPort("_exit");

            EditorGUILayout.BeginHorizontal();
            if (enterPort != null) NodeEditorGUILayout.PortField(new GUIContent("Enter"), enterPort, GUILayout.MinWidth(0));
            GUILayout.FlexibleSpace();
            if (exitPort != null) NodeEditorGUILayout.PortField(new GUIContent("Exit"), exitPort, GUILayout.MinWidth(0));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(8);
            DrawOperationsSection();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader()
        {
            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(EditorStyles.miniLabel);
                _headerStyle.alignment = TextAnchor.UpperRight;
                _headerStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
            }

            SerializedProperty nameProp = serializedObject.FindProperty("_name");
            SerializedProperty guidProp = serializedObject.FindProperty("_guid");

            if (guidProp != null)
            {
                GUILayout.Label($"ID: {guidProp.stringValue.Substring(0, 8)}...", _headerStyle);
            }

            EditorGUILayout.PropertyField(nameProp, new GUIContent("Method Name"));
        }

        private void DrawOperationsSection()
        {
            if (_bodyStyle == null)
            {
                _bodyStyle = new GUIStyle(EditorStyles.helpBox);
                _bodyStyle.padding = new RectOffset(10, 10, 10, 10);
            }

            XNode.NodePort opsPort = target.GetPort("_operations");

            GUILayout.BeginVertical(_bodyStyle);

            EditorGUILayout.LabelField("⚡ OPERATIONS", EditorStyles.miniBoldLabel);
            GUILayout.Space(4);

            if (opsPort != null)
            {
                NodeEditorGUILayout.PortField(GUIContent.none, opsPort, GUILayout.MinWidth(0));
            }

            GUILayout.EndVertical();
        }

        public override int GetWidth()
        {
            return 220;
        }

        public override Color GetTint()
        {
            return new Color(0.29f, 0.2f, 0.35f);
        }
    }
}
#endif