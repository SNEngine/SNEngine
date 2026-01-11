using SiphoinUnityHelpers.XNodeExtensions;
using SNEngine.Graphs;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using XNode;

public static class DialogueCreatorEditor
{
    private const string TemplatePath = "Assets/SNEngine/Source/SNEngine/Editor/Templates/DialogueTemplate.asset";
    private const string TargetFolderPath = "Assets/SNEngine/Source/SNEngine/Resources/Dialogues";
    private const string MenuItemPath = "SNEngine/New Dialogue";
    private const string RegenerateMethodName = "ResetGUID";
    private const string NodeEditorWindowTypeName = "XNodeEditor.NodeEditorWindow";

    [MenuItem(MenuItemPath)]
    public static void CreateNewDialogueAsset()
    {
        CreateNewDialogueAssetFromName(GetUniqueDialogueAssetName());
    }

    public static void CreateNewDialogueAssetFromName(string assetName)
    {
        string finalAssetName = assetName.EndsWith(".asset") ? assetName : $"{assetName}.asset";
        string fullTemplatePath = Path.Combine(Application.dataPath, TemplatePath.Replace("Assets/", ""));

        if (!File.Exists(fullTemplatePath))
        {
            Debug.LogError($"[DialogueCreator] Template not found at path: {TemplatePath} (Checked: {fullTemplatePath})");
            return;
        }

        if (!Directory.Exists(TargetFolderPath))
        {
            Directory.CreateDirectory(TargetFolderPath);
        }

        string newAssetPath = Path.Combine(TargetFolderPath, finalAssetName);

        if (AssetDatabase.CopyAsset(TemplatePath, newAssetPath))
        {
            AssetDatabase.Refresh();

            NodeGraph dialogueGraph = AssetDatabase.LoadAssetAtPath<NodeGraph>(newAssetPath);

            if (dialogueGraph != null)
            {
                if (dialogueGraph is BaseGraph baseGraph)
                {
#if UNITY_EDITOR
                    RegenerateGraphGuid(baseGraph);
#endif
                }

                RegenerateAllNodeGuids(dialogueGraph);

                EditorUtility.SetDirty(dialogueGraph);
                AssetDatabase.SaveAssets();

                OpenGraph(dialogueGraph);
                Selection.activeObject = dialogueGraph;
                Debug.Log($"[DialogueCreator] New dialogue created: {finalAssetName}");
            }
            else
            {
                Debug.LogError($"[DialogueCreator] Failed to load copied asset: {newAssetPath}");
            }
        }
        else
        {
            Debug.LogError($"[DialogueCreator] Failed to copy asset from {TemplatePath} to {newAssetPath}");
        }
    }

    public static void DuplicateDialogue(DialogueGraph sourceGraph, string newName)
    {
        if (sourceGraph == null) return;

        string sourcePath = AssetDatabase.GetAssetPath(sourceGraph);
        string finalAssetName = newName.EndsWith(".asset") ? newName : $"{newName}.asset";
        string newAssetPath = Path.Combine(TargetFolderPath, finalAssetName);

        if (File.Exists(Path.Combine(Application.dataPath, newAssetPath.Replace("Assets/", ""))))
        {
            Debug.LogError($"[DialogueCreator] Asset already exists at: {newAssetPath}");
            return;
        }

        if (AssetDatabase.CopyAsset(sourcePath, newAssetPath))
        {
            AssetDatabase.Refresh();
            DialogueGraph newGraph = AssetDatabase.LoadAssetAtPath<DialogueGraph>(newAssetPath);

            if (newGraph != null)
            {
                RegenerateGraphGuid(newGraph);

                RegenerateAllNodeGuids(newGraph);

                EditorUtility.SetDirty(newGraph);
                AssetDatabase.SaveAssets();

                Debug.Log($"[DialogueCreator] Dialogue duplicated: {finalAssetName}");
            }
        }
    }

    private static void RegenerateGraphGuid(BaseGraph graph)
    {
        var regenerateMethod = typeof(BaseGraph).GetMethod(RegenerateMethodName,
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (regenerateMethod != null)
        {
            regenerateMethod.Invoke(graph, null);
            EditorUtility.SetDirty(graph);
        }
    }

    public static bool RenameDialogueAsset(NodeGraph graph, string newName)
    {
        string assetPath = AssetDatabase.GetAssetPath(graph);
        if (string.IsNullOrEmpty(assetPath))
        {
            Debug.LogError($"[DialogueCreator] Could not find asset path for: {graph.name}");
            return false;
        }

        if (newName.EndsWith(".asset"))
        {
            newName = newName.Substring(0, newName.Length - 6);
        }

        string error = AssetDatabase.RenameAsset(assetPath, newName);

        if (string.IsNullOrEmpty(error))
        {
            AssetDatabase.Refresh();
            Debug.Log($"[DialogueCreator] Dialogue renamed to: {newName}");
            return true;
        }
        else
        {
            Debug.LogError($"[DialogueCreator] Renaming failed: {error}");
            return false;
        }
    }

    public static void OpenGraph(NodeGraph graph)
    {
        if (graph == null) return;

        XNodeEditor.NodeEditorWindow.Open(graph);
    }

    private static string GetUniqueDialogueAssetName()
    {
        string targetPath = Path.Combine(Application.dataPath, TargetFolderPath.Replace("Assets/", ""));

        if (!Directory.Exists(targetPath))
        {
            return "_startDialogue.asset";
        }

        string[] existingFiles = Directory.GetFiles(targetPath, "*.asset");

        if (!existingFiles.Any(f => Path.GetFileName(f).Equals("_startDialogue.asset", System.StringComparison.OrdinalIgnoreCase)))
        {
            return "_startDialogue.asset";
        }


        int maxNumber = 0;
        var regex = new Regex(@"dialogue_(\d+)\.asset");

        foreach (string file in existingFiles)
        {
            string fileName = Path.GetFileName(file);
            Match match = regex.Match(fileName);

            if (match.Success)
            {
                if (int.TryParse(match.Groups[1].Value, out int number))
                {
                    if (number > maxNumber)
                    {
                        maxNumber = number;
                    }
                }
            }
        }

        return $"dialogue_{maxNumber + 1}.asset";
    }

    private static void RegenerateAllNodeGuids(NodeGraph graph)
    {
        foreach (Node node in graph.nodes)
        {
            if (node is BaseNode baseNode)
            {
                var regenerateMethod = typeof(BaseNode).GetMethod(RegenerateMethodName,
                    BindingFlags.NonPublic | BindingFlags.Instance);

                if (regenerateMethod != null)
                {
                    regenerateMethod.Invoke(baseNode, null);
                    EditorUtility.SetDirty(baseNode);
                }
            }
        }
    }
}