using UnityEditor;
using UnityEngine;
using XNodeEditor;
using SNEngine.SelectVariantsSystem;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(ShowVariantsNode))]
    public class ShowVariantsNodeEditor : NodeEditor
    {
        private GUIStyle _variantBoxStyle;
        private GUIStyle _textInputStyle;

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            DrawPort("_enter");
            DrawCompactSettings();

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Choice Variants", EditorStyles.boldLabel);

            DrawDynamicVariants();

            DrawOutputResult();
            DrawPort("_exit");

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPort(string fieldName)
        {
            XNode.NodePort port = target.GetPort(fieldName);
            if (port != null) NodeEditorGUILayout.PortField(port);
        }

        private void DrawCompactSettings()
        {
            SerializedProperty animType = serializedObject.FindProperty("_typeAnimation");

            float prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;
            NodeEditorGUILayout.PropertyField(animType, new GUIContent("Type Animation"));
            EditorGUIUtility.labelWidth = prevLabelWidth;

            GUILayout.Space(5);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            DrawBoolToggle("_hideCharacters", "Hide Characters");
            DrawBoolToggle("_hideDialogWindow", "Hide Dialog Window");
            DrawBoolToggle("_returnCharacterVisible", "Return Char Visible");
            GUILayout.EndVertical();
        }

        private void DrawBoolToggle(string propertyName, string label)
        {
            SerializedProperty prop = serializedObject.FindProperty(propertyName);
            if (prop == null) return;

            EditorGUILayout.BeginHorizontal();
            prop.boolValue = EditorGUILayout.ToggleLeft(label, prop.boolValue, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawDynamicVariants()
        {
            SerializedProperty variantsProp = serializedObject.FindProperty("_variants");
            int indexToDelete = -1;

            if (_variantBoxStyle == null)
            {
                _variantBoxStyle = new GUIStyle(EditorStyles.helpBox);
                _variantBoxStyle.padding = new RectOffset(5, 5, 5, 5);

                _textInputStyle = new GUIStyle(EditorStyles.textArea);
                _textInputStyle.wordWrap = true;
                _textInputStyle.richText = false;
                _textInputStyle.normal.background = null;
                _textInputStyle.focused.background = null;
            }

            for (int i = 0; i < variantsProp.arraySize; i++)
            {
                SerializedProperty element = variantsProp.GetArrayElementAtIndex(i);

                GUILayout.BeginVertical(_variantBoxStyle);

                EditorGUILayout.BeginHorizontal();
                GUI.color = Color.gray;
                EditorGUILayout.LabelField($"#{i + 1}", EditorStyles.miniLabel, GUILayout.Width(20));
                GUI.color = Color.white;

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("✕", EditorStyles.miniButton, GUILayout.Width(18), GUILayout.Height(14)))
                {
                    indexToDelete = i;
                }
                EditorGUILayout.EndHorizontal();

                element.stringValue = EditorGUILayout.TextArea(
                    element.stringValue,
                    _textInputStyle,
                    GUILayout.ExpandHeight(false)
                );

                GUILayout.EndVertical();
                GUILayout.Space(4);
            }

            if (indexToDelete != -1)
            {
                variantsProp.DeleteArrayElementAtIndex(indexToDelete);
            }

            if (GUILayout.Button("+ Add New Choice", GUILayout.Height(22)))
            {
                variantsProp.arraySize++;
            }
        }

        private void DrawOutputResult()
        {
            GUILayout.Space(8);
            XNode.NodePort resultPort = target.GetOutputPort("_selectedIndex");
            if (resultPort != null)
            {
                NodeEditorGUILayout.PortField(new GUIContent("Result Selection"), resultPort);
            }
        }

        public override Color GetTint()
        {
            return new Color(0.15f, 0.18f, 0.22f);
        }
    }
}