using UnityEditor;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using SNEngine.Debugging;
namespace SNEngine.Editor
{
    public class YamlEditorLauncher : BaseToolLauncher
    {
        [MenuItem("SNEngine/Editors/Yaml Editor")]
        public static void Launch()
        {
            LaunchExecutable("Yaml Editor", "YAML Editor.exe", "YAMLEditor");
        }
    }
}