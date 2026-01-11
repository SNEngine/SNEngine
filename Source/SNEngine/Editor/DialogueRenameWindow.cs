using SNEngine.Graphs;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor
{
    public class DialogueRenameWindow : BaseRenameWindow<DialogueGraph>
    {
        public static void OpenWindow(DialogueGraph asset)
        {
            var window = CreateInstance<DialogueRenameWindow>();
            window.targetAsset = asset;
            window.newName = asset.name;

            window.titleContent = new GUIContent("Rename Dialogue");

            Vector2 size = new Vector2(400, 170);
            window.minSize = window.maxSize = size;

            Rect mainEditorRect = EditorGUIUtility.GetMainWindowPosition();
            float centerX = mainEditorRect.x + (mainEditorRect.width - size.x) / 2;
            float centerY = mainEditorRect.y + (mainEditorRect.height - size.y) / 2;

            window.position = new Rect(centerX, centerY, size.x, size.y);

            window.ShowModalUtility();
        }
    }
}