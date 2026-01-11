using SNEngine.Graphs;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor
{
    public class DialogueDuplicateWindow : BaseRenameWindow<DialogueGraph>
    {
        public static void Open(DialogueGraph asset)
        {
            var window = CreateInstance<DialogueDuplicateWindow>();
            window.targetAsset = asset;
            window.newName = asset.name + "_copy";

            window.titleContent = new GUIContent("Duplicate Dialogue");

            Vector2 size = new Vector2(400, 160);
            window.minSize = window.maxSize = size;

            // Центрирование окна
            Rect mainEditorRect = EditorGUIUtility.GetMainWindowPosition();
            float centerX = mainEditorRect.x + (mainEditorRect.width - size.x) / 2;
            float centerY = mainEditorRect.y + (mainEditorRect.height - size.y) / 2;
            window.position = new Rect(centerX, centerY, size.x, size.y);

            window.ShowModalUtility();
        }

        protected override void DrawButtons()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel", GUILayout.Width(90), GUILayout.Height(24)))
            {
                this.Close();
                GUIUtility.ExitGUI();
            }

            GUI.backgroundColor = new Color(0.1f, 0.6f, 0.1f); // Зеленый стиль для дубликации
            if (GUILayout.Button("Duplicate", GUILayout.Width(90), GUILayout.Height(24)))
            {
                ApplyAction();
                GUIUtility.ExitGUI();
            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(15);
            EditorGUILayout.EndHorizontal();
        }

        protected override void ApplyAction()
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                EditorUtility.DisplayDialog("Error", "Please enter a valid name.", "OK");
                return;
            }

            DialogueCreatorEditor.DuplicateDialogue(targetAsset, newName);
            this.Close();
        }
    }
}