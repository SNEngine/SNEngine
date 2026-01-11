#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using SNEngine.DialogOnScreenSystem;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(DialogOnScreenNode))]
    public class DialogOnScreenNodeEditor : NodeEditor
    {
        private GUIStyle _textInputStyle;

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            foreach (var tag in NodeEditorGUILayout.GetFilteredFields(serializedObject))
            {
                if (tag.name == "_text" || tag.name == "m_Script") continue;
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(tag.name));
            }

            DrawDialogueField();

            GUILayout.Space(10);
            serializedObject.ApplyModifiedProperties();
        }


        private void DrawDialogueField()
        {
            if (_textInputStyle == null)
            {
                _textInputStyle = new GUIStyle(EditorStyles.textArea);
                _textInputStyle.wordWrap = true;
                _textInputStyle.richText = false;
                _textInputStyle.normal.background = null;
                _textInputStyle.focused.background = null;
            }

            SerializedProperty textProp = serializedObject.FindProperty("_text");
            if (textProp == null) return;

            EditorGUILayout.LabelField("Text Content", EditorStyles.boldLabel);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            textProp.stringValue = EditorGUILayout.TextArea(
                textProp.stringValue,
                _textInputStyle,
                GUILayout.ExpandHeight(false)
            );
            GUILayout.EndVertical();
        }

        public override Color GetTint() => new Color(0.2f, 0.2f, 0.3f);
    }
}
#endif