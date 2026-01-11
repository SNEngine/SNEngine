#if UNITY_EDITOR
using SiphoinUnityHelpers.XNodeExtensions.Editor;
using SNEngine.DialogSystem;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(DialogNode))]
    public class DialogNodeEditor : NodeEditor
    {
        private GUIStyle _textInputStyle;

        public override void OnBodyGUI()
        {
            serializedObject.Update();
            DialogNode node = target as DialogNode;

            foreach (var tag in NodeEditorGUILayout.GetFilteredFields(serializedObject))
            {
                if (tag.name == "_character" || tag.name == "_text" || tag.name == "m_Script") continue;
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(tag.name));
            }

            GUILayout.Space(5);
            DrawDialogueField();
            GUILayout.Space(10);
            DrawCharacterSelector(node);

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

            EditorGUILayout.LabelField("Dialogue Text", EditorStyles.boldLabel);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            textProp.stringValue = EditorGUILayout.TextArea(
                textProp.stringValue,
                _textInputStyle,
                GUILayout.ExpandHeight(false)
            );
            GUILayout.EndVertical();
        }

        private void DrawCharacterSelector(DialogNode node)
        {
            string charName = node.Character != null ? node.Character.name : "Select Character";
            GUIContent content = new GUIContent(charName);
            Color prevBg = GUI.backgroundColor;
            GUI.backgroundColor = node.Character != null ? new Color(0.4f, 0.75f, 0.45f) : new Color(0.75f, 0.4f, 0.4f);

            if (GUILayout.Button(content, GUILayout.Height(32)))
            {
                CharacterSelectorWindow.Open((selected) => {
                    var so = new SerializedObject(target);
                    var p = so.FindProperty("_character");
                    if (p != null) { p.objectReferenceValue = selected; so.ApplyModifiedProperties(); }
                });
            }
            GUI.backgroundColor = prevBg;
        }
    }
}
#endif