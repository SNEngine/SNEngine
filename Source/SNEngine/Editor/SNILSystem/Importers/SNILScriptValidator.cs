using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SNEngine.Editor.SNILSystem.Parsers;
using SNEngine.Editor.SNILSystem.FunctionSystem;
using UnityEngine;
using UnityEditor;
using SNEngine.Graphs;

namespace SNEngine.Editor.SNILSystem.Importers
{
    public class SNILScriptValidator
    {
        public static bool ValidateScript(string filePath, out List<Validators.SNILValidationError> errors)
        {
            errors = new List<Validators.SNILValidationError>();

            try
            {
                filePath = filePath.Trim().Trim('"', '@', '\'');

                if (!File.Exists(filePath))
                {
                    errors.Add(new Validators.SNILValidationError
                    {
                        LineNumber = 0,
                        LineContent = "",
                        ErrorType = Validators.SNILValidationErrorType.EmptyFile,
                        Message = $"File not found: {filePath}"
                    });
                    return false;
                }

                List<string[]> scriptParts = SNILMultiScriptParser.ParseMultiScript(filePath);

                bool isValid = true;
                foreach (string[] part in scriptParts)
                {
                    Validators.SNILSyntaxValidator validator = new Validators.SNILSyntaxValidator();
                    if (!validator.Validate(part, out string errorMessage, out List<Validators.SNILValidationError> partErrors))
                    {
                        errors.AddRange(partErrors);
                        isValid = false;
                    }
                }

                return isValid;
            }
            catch (Exception e)
            {
                errors.Add(new Validators.SNILValidationError
                {
                    LineNumber = 0,
                    LineContent = "",
                    ErrorType = Validators.SNILValidationErrorType.EmptyFile,
                    Message = $"Import failed: {e.Message}"
                });
                return false;
            }
        }

        public static List<string> GetAllGraphNamesInFile(string filePath)
        {
            List<string> graphNames = new List<string>();

            if (!File.Exists(filePath))
            {
                return graphNames;
            }

            var scriptParts = SNILMultiScriptParser.ParseMultiScript(filePath);

            foreach (string[] part in scriptParts)
            {
                var functions = SNILFunctionParser.ParseFunctions(part);
                var mainScriptLines = SNILFunctionParser.ExtractMainScriptWithoutFunctions(part).ToArray();

                string graphName = "NewGraph";
                foreach (string line in mainScriptLines)
                {
                    var nameMatch = Regex.Match(line.Trim(), @"^name:\s*(.+)", RegexOptions.IgnoreCase);
                    if (nameMatch.Success)
                    {
                        graphName = nameMatch.Groups[1].Value.Trim();
                        break;
                    }
                }

                graphNames.Add(graphName);
            }

            return graphNames;
        }

        public static void CreateAllGraphsInFile(string filePath)
        {
            var scriptParts = SNILMultiScriptParser.ParseMultiScript(filePath);

            foreach (string[] part in scriptParts)
            {
                var functions = SNILFunctionParser.ParseFunctions(part);
                var mainScriptLines = SNILFunctionParser.ExtractMainScriptWithoutFunctions(part).ToArray();

                string graphName = "NewGraph";
                foreach (string line in mainScriptLines)
                {
                    var nameMatch = Regex.Match(line.Trim(), @"^name:\s*(.+)", RegexOptions.IgnoreCase);
                    if (nameMatch.Success)
                    {
                        graphName = nameMatch.Groups[1].Value.Trim();
                        break;
                    }
                }

                graphName = SanitizeFileName(graphName);

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
        }

        private static string SanitizeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars()) fileName = fileName.Replace(c, '_');
            return string.IsNullOrWhiteSpace(fileName) ? "NewGraph" : fileName;
        }
    }
}