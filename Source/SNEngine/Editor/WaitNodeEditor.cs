#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using SiphoinUnityHelpers.XNodeExtensions.AsyncNodes;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(WaitNode))]
    public class WaitNodeEditor : NodeEditor
    {
        private GUIStyle _timeStyle;
        private GUIStyle _boxStyle;

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            DrawPort("_enter");
            GUILayout.Space(5);
            DrawTimeField();
            GUILayout.Space(5);
            DrawPort("_exit");

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPort(string fieldName)
        {
            XNode.NodePort port = target.GetPort(fieldName);
            if (port != null) NodeEditorGUILayout.PortField(port);
        }

        private void DrawTimeField()
        {
            if (_timeStyle == null)
            {
                _timeStyle = new GUIStyle(EditorStyles.boldLabel);
                _timeStyle.fontSize = 18;
                _timeStyle.alignment = TextAnchor.MiddleCenter;
                _timeStyle.normal.textColor = new Color(0.4f, 0.8f, 1f);
            }

            if (_boxStyle == null)
            {
                _boxStyle = new GUIStyle(EditorStyles.helpBox);
                _boxStyle.padding = new RectOffset(10, 10, 10, 10);
            }

            SerializedProperty secondsProp = serializedObject.FindProperty("_seconds");
            XNode.NodePort secondsPort = target.GetPort("_seconds");

            GUILayout.BeginVertical(_boxStyle);
            EditorGUILayout.LabelField("⏱ Wait Time", EditorStyles.miniLabel);

            if (secondsPort != null)
            {
                NodeEditorGUILayout.PortField(new GUIContent("Seconds Source"), secondsPort);
            }

            float displayValue = secondsProp.floatValue;

            if (secondsPort != null && secondsPort.IsConnected)
            {
                object val = secondsPort.GetInputValue();
                if (val is float f) displayValue = f;
                else if (val is int i) displayValue = i;

                GUI.enabled = false;
                EditorGUILayout.TextField("Connected Value", displayValue.ToString("F2"));
                GUI.enabled = true;
            }
            else
            {
                secondsProp.floatValue = EditorGUILayout.FloatField("Manual Value", secondsProp.floatValue);
                displayValue = secondsProp.floatValue;
            }

            GUILayout.Label($"{displayValue:F2}s", _timeStyle);

            GUILayout.EndVertical();
        }

        public override Color GetTint()
        {
            return new Color(0.1f, 0.25f, 0.35f);
        }
    }
}
#endif