using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using SNEngine.Graphs;
using SNEngine.Editor.SNILSystem.FunctionSystem;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem.Importers
{
    public class SNILGraphCreator
    {
        public static void CreateSingleGraph(string[] lines)
        {
            if (lines.Length == 0) return;

            Validators.SNILSyntaxValidator validator = new Validators.SNILSyntaxValidator();
            if (!validator.Validate(lines, out string errorMessage))
            {
                SNILDebug.LogError($"SNIL script validation failed: {errorMessage}");
                return;
            }

            string graphName = ExtractGraphName(lines);
            graphName = SanitizeFileName(graphName);

            CreateGraphAsset(graphName);
        }

        public static void CreateGraphAsset(string graphName)
        {
            string assetPath = $"Assets/SNEngine/Source/SNEngine/Resources/Dialogues/{graphName}.asset";
            DialogueGraph graph = AssetDatabase.LoadAssetAtPath<DialogueGraph>(assetPath);

            if (graph == null)
            {
                graph = ScriptableObject.CreateInstance<DialogueGraph>();
                graph.name = graphName;

                string folderPath = "Assets/SNEngine/Source/SNEngine/Resources/Dialogues";
                if (!AssetDatabase.IsValidFolder("Assets/SNEngine")) AssetDatabase.CreateFolder("Assets", "SNEngine");
                if (!AssetDatabase.IsValidFolder("Assets/SNEngine/Source")) AssetDatabase.CreateFolder("Assets/SNEngine", "Source");
                if (!AssetDatabase.IsValidFolder("Assets/SNEngine/Source/SNEngine")) AssetDatabase.CreateFolder("Assets/SNEngine/Source", "SNEngine");
                if (!AssetDatabase.IsValidFolder("Assets/SNEngine/Source/SNEngine/Resources")) AssetDatabase.CreateFolder("Assets/SNEngine/Source/SNEngine", "Resources");
                if (!AssetDatabase.IsValidFolder(folderPath)) AssetDatabase.CreateFolder("Assets/SNEngine/Source/SNEngine/Resources", "Dialogues");

                AssetDatabase.CreateAsset(graph, assetPath);
                AssetDatabase.SaveAssets();
            }

            SNILPostProcessor.RegisterGraph(graphName, graph);
        }

        public static string ExtractGraphName(string[] lines)
        {
            string graphName = "NewGraph";
            foreach (string line in lines)
            {
                var nameMatch = Regex.Match(line.Trim(), @"^name:\s*(.+)", RegexOptions.IgnoreCase);
                if (nameMatch.Success)
                {
                    graphName = nameMatch.Groups[1].Value.Trim();
                    break;
                }
            }
            return graphName;
        }

        private static string SanitizeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars()) fileName = fileName.Replace(c, '_');
            return string.IsNullOrWhiteSpace(fileName) ? "NewGraph" : fileName;
        }
    }
}