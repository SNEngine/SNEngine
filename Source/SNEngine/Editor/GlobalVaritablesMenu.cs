using UnityEditor;
using XNodeEditor;
using SNEngine.Graphs;
using SNEngine.Debugging;

namespace SNEngine.Editor
{
    public static class GlobalVariablesMenu
    {
        [MenuItem("SNEngine/Open Global Variables Window")]
        public static void OpenGlobalVariables()
        {
            string path = "Assets/SNEngine/Source/SNEngine/Resources/VariableContainerGraph.asset";
            VariableContainerGraph graph = AssetDatabase.LoadAssetAtPath<VariableContainerGraph>(path);

            if (graph != null)
            {
                NodeEditorWindow.Open(graph);
            }
            else
            {
               NovelGameDebug.LogError($"[SNEngine] Global Variables not found: {path}");
            }
        }
    }
}