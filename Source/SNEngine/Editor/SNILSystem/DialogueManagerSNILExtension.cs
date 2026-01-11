using System.IO;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem
{
    public static class DialogueManagerSNILExtension
    {
        [MenuItem("SNEngine/SNIL/Import SNIL Script", priority = 101)]
        public static void ImportSNILScriptFromMenu()
        {
            string selectedPath = EditorUtility.OpenFilePanel(
                "Select SNIL Script",
                "",
                "snil"
            );

            if (!string.IsNullOrEmpty(selectedPath))
            {
                // Extract graph name from filename if not specified in the file
                string graphName = Path.GetFileNameWithoutExtension(selectedPath);

                // Call the SNIL compiler to import the script
                bool success = SNILCompiler.ImportScript(selectedPath);

                // Refresh the asset database
                AssetDatabase.Refresh();

                if (success)
                {
                    SNILDebug.Log($"SNIL script imported successfully as '{graphName}'!");
                }
                else
                {
                    SNILDebug.LogError($"Failed to import SNIL script '{graphName}'. Check console for errors.");
                }
            }
        }
    }
}