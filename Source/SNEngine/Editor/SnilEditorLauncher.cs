using UnityEditor;
namespace SNEngine.Editor
{
    public class SnilEditorLauncher : BaseToolLauncher
    {
        [MenuItem("SNEngine/Editors/SNIL Editor")]
        public static void Launch()
        {
            LaunchExecutable("SNIL Editor", "SNIL Editor.exe", "SNILEditor");
        }
    }
}