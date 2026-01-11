#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using XNodeEditor;
using SNEngine.Graphs;
using SiphoinUnityHelpers.XNodeExtensions;

namespace SNEngine.Editor
{
    [CustomNodeEditor(typeof(JumpToDialogueNode))]
    public class JumpToDialogueNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            JumpToDialogueNode node = target as JumpToDialogueNode;

            DrawPorts(node);

            GUILayout.Space(5);

            DrawDialogueSelector(node);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPorts(JumpToDialogueNode node)
        {
            EditorGUILayout.BeginHorizontal();

            var enterPort = node.GetEnterPort();
            if (enterPort != null) NodeEditorGUILayout.PortField(new GUIContent("Enter"), enterPort);

            var exitPort = node.GetExitPort();
            if (exitPort != null) NodeEditorGUILayout.PortField(new GUIContent("Exit"), exitPort, GUILayout.MinWidth(0));

            EditorGUILayout.EndHorizontal();
        }

        private void DrawDialogueSelector(JumpToDialogueNode node)
        {
            SerializedProperty dialogueProp = serializedObject.FindProperty("_dialogue");
            DialogueGraph currentDialogue = dialogueProp.objectReferenceValue as DialogueGraph;

            Color prevBg = GUI.backgroundColor;

            GUI.backgroundColor = currentDialogue != null ? new Color(0.4f, 0.1f, 0.1f) : new Color(0.75f, 0.4f, 0.4f);

            EditorGUILayout.BeginVertical(GUI.skin.box);

            float rowHeight = currentDialogue != null ? 50f : 34f;
            Rect rect = GUILayoutUtility.GetRect(10, rowHeight);

            if (GUI.Button(rect, currentDialogue == null ? "Assign Dialogue Graph" : ""))
            {
                DialogueSelectorWindow.Open((selected) => {
                    var so = new SerializedObject(target);
                    so.FindProperty("_dialogue").objectReferenceValue = selected;
                    so.ApplyModifiedProperties();
                });
            }

            if (currentDialogue != null)
            {
                string path = AssetDatabase.GetAssetPath(currentDialogue);
                Texture icon = AssetDatabase.GetCachedIcon(path);

                Rect iconRect = new Rect(rect.x + 8, rect.y + 12, 26, 26);
                if (icon != null) GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);

                Rect textRect = new Rect(rect.x + 40, rect.y + 8, rect.width - 45, 20);
                GUI.Label(textRect, currentDialogue.name, EditorStyles.boldLabel);

                Rect pathRect = new Rect(rect.x + 40, rect.y + 26, rect.width - 45, 16);
                GUI.Label(pathRect, "Dialogue Graph", EditorStyles.miniLabel);
            }

            EditorGUILayout.EndVertical();
            GUI.backgroundColor = prevBg;
        }
    }
}
#endif