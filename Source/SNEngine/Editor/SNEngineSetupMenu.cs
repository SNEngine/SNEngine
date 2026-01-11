using UnityEngine;
using UnityEditor;

namespace SNEngine.Editor
{
    public static class SNEngineSetupMenu
    {
        [MenuItem("SNEngine/Setup", false, 0)]
        public static void OpenSetupWindow()
        {
            WelcomeWindow window = EditorWindow.GetWindow<WelcomeWindow>(true, "SNEngine Setup", true);
            window.minSize = new Vector2(500, 450);
            window.ShowUtility();
        }
    }
}