#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using SiphoinUnityHelpers.XNodeExtensions.Web;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(GetTextRequestNode))]
    public class GetTextRequestNodeEditor : NodeEditor
    {
        private bool _showPreview = true;

        public override void OnBodyGUI()
        {
            serializedObject.Update();
            GetTextRequestNode node = target as GetTextRequestNode;

            foreach (var tag in NodeEditorGUILayout.GetFilteredFields(serializedObject))
            {
                if (tag.name == "_text") continue;
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(tag.name));
            }

            SerializedProperty textProp = serializedObject.FindProperty("_text");
            string content = textProp.stringValue;

            if (!string.IsNullOrEmpty(content))
            {
                GUILayout.Space(5);
                _showPreview = EditorGUILayout.Foldout(_showPreview, "Response Preview");

                if (_showPreview)
                {
                    GUIStyle style = new GUIStyle(EditorStyles.textArea);
                    style.wordWrap = true;
                    style.fontSize = 9;

                    float height = Mathf.Min(100, style.CalcHeight(new GUIContent(content), 180));
                    EditorGUILayout.SelectableLabel(content, style, GUILayout.Height(height));
                }
            }

            NodeEditorGUILayout.PortField(new GUIContent("Downloaded Text"), node.GetOutputPort("_text"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif