using UnityEditor;

namespace SNEngine.Editor
{
    public static class EditorWindowExtensions
    {
        public static void CenterOnMainWin(this EditorWindow window)
        {
            var main = EditorGUIUtility.GetMainWindowPosition();
            var pos = window.position;
            float w = (main.width - pos.width) * 0.5f;
            float h = (main.height - pos.height) * 0.5f;
            pos.x = main.x + w;
            pos.y = main.y + h;
            window.position = pos;
        }
    }
}