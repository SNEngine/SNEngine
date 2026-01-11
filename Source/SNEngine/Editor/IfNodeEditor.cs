#if UNITY_EDITOR
using SiphoinUnityHelpers.XNodeExtensions.NodesControlExecutes;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(IfNode))]
    public class IfNodeEditor : NodeEditor
    {
        private GUIStyle _conditionStyle;
        private GUIStyle _centeredLabel;

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            XNode.NodePort enterPort = target.GetPort("_enter");
            if (enterPort != null) NodeEditorGUILayout.PortField(enterPort);

            GUILayout.Space(5);
            DrawConditionSection();

            GUILayout.Space(5);
            DrawBranchSection("✔ TRUE", "_true", Color.green);
            GUILayout.Space(2);
            DrawBranchSection("✖ FALSE", "_false", Color.red);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawConditionSection()
        {
            if (_conditionStyle == null)
            {
                _conditionStyle = new GUIStyle(EditorStyles.helpBox);
                _conditionStyle.padding = new RectOffset(10, 10, 10, 10);
            }

            XNode.NodePort condPort = target.GetPort("_condition");
            SerializedProperty condProp = serializedObject.FindProperty("_condition");

            GUILayout.BeginVertical(_conditionStyle);
            EditorGUILayout.LabelField("❓ Condition", EditorStyles.miniBoldLabel);

            if (condPort != null)
            {
                NodeEditorGUILayout.PortField(new GUIContent("In"), condPort);
            }

            if (Application.isPlaying && condPort != null && condPort.IsConnected)
            {
                bool displayValue = condProp.boolValue;
                try
                {
                    object val = condPort.GetInputValue();
                    if (val is bool b) displayValue = b;
                }
                catch { }

                GUI.enabled = false;
                EditorGUILayout.Toggle("External", displayValue);
                GUI.enabled = true;

                if (_centeredLabel == null)
                {
                    _centeredLabel = new GUIStyle(EditorStyles.boldLabel);
                    _centeredLabel.alignment = TextAnchor.MiddleCenter;
                    _centeredLabel.fontSize = 13;
                }
                _centeredLabel.normal.textColor = displayValue ? new Color(0.4f, 1f, 0.4f) : new Color(1f, 0.4f, 0.4f);
                GUILayout.Label(displayValue ? "TRUE" : "FALSE", _centeredLabel);
            }
            else if (!condPort.IsConnected)
            {
                condProp.boolValue = EditorGUILayout.Toggle("Manual", condProp.boolValue);
            }

            GUILayout.EndVertical();
        }

        public override Color GetTint()
        {
            return new Color(0.25f, 0.25f, 0.25f);
        }

        private void DrawBranchSection(string label, string portName, Color color)
        {
            XNode.NodePort port = target.GetPort(portName);
            if (port == null) return;

            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();

            GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
            labelStyle.normal.textColor = color;

            GUILayout.Label(label, labelStyle);
            GUILayout.FlexibleSpace();

            NodeEditorGUILayout.PortField(GUIContent.none, port, GUILayout.Width(20));

            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        public override int GetWidth()
        {
            return 150;
        }

    }
}
#endif