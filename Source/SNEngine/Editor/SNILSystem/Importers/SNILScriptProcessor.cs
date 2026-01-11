using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SNEngine.Graphs;
using SNEngine.Editor.SNILSystem.FunctionSystem;
using SNEngine.Editor.SNILSystem.Parsers;
using SNEngine.Editor.SNILSystem.InstructionHandlers;
using UnityEditor;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem.Importers
{
    public class SNILScriptProcessor
    {
        public static void ProcessSingleGraph(string[] lines)
        {
            if (lines.Length == 0) return;

            string graphName = ExtractGraphName(lines);
            graphName = SanitizeFileName(graphName);

            string assetPath = $"Assets/SNEngine/Source/SNEngine/Resources/Dialogues/{graphName}.asset";
            DialogueGraph graph = AssetDatabase.LoadAssetAtPath<DialogueGraph>(assetPath);

            if (graph == null)
            {
                SNILDebug.LogError($"Could not load graph: {assetPath}");
                return;
            }

            var functions = SNILFunctionParser.ParseFunctions(lines);
            var mainScriptLines = SNILFunctionParser.ExtractMainScriptWithoutFunctions(lines).ToArray();

            var functionInstructions = ParseFunctionInstructions(functions, graph);

            var (mainInstructions, functionCallPositions, functionCallNames) = ParseScriptWithFunctionCalls(mainScriptLines, graph);

            ApplyInstructionsToGraph(graphName, mainInstructions, functionInstructions, functionCallPositions, functionCallNames);
        }

        public static List<SNILInstruction> ParseFunctionInstructions(List<SNILFunction> functions, DialogueGraph graph)
        {
            var functionInstructions = new List<SNILInstruction>();

            foreach (var function in functions)
            {
                // Создаем GroupCallsNode для функции
                var groupCallsNodeInstruction = new SNILInstruction
                {
                    Type = SNILInstructionType.Generic,
                    NodeTypeName = "GroupCallsNode",
                    Parameters = new Dictionary<string, string> { { "name", function.Name } },
                    NodeType = SNILTypeResolver.GetNodeType("GroupCallsNode")
                };

                functionInstructions.Add(groupCallsNodeInstruction);

                // Создаем ноды для тела функции
                var functionBodyInstructions = ParseScript(function.Body, graph);

                // Добавляем инструкции тела функции
                foreach (var instruction in functionBodyInstructions)
                {
                    functionInstructions.Add(instruction);
                }
            }

            return functionInstructions;
        }

        public static (List<SNILInstruction>, List<int>, List<string>) ParseScriptWithFunctionCalls(string[] lines, DialogueGraph graph)
        {
            var (instructions, calls) = ParseLinesToInstructions(lines, graph);

            var callPositions = new List<int>();
            var callNames = new List<string>();

            foreach (var c in calls)
            {
                callPositions.Add(c.position);
                callNames.Add(c.name);
            }

            return (instructions, callPositions, callNames);
        }

        private static (List<SNILInstruction> instructions, List<(int position, string name)> calls) ParseLinesToInstructions(string[] lines, DialogueGraph graph)
        {
            var templates = SNILTemplateManager.GetNodeTemplates();
            var handlerManager = InstructionHandlerManager.Instance;

            var instructions = new List<SNILInstruction>();
            var calls = new List<(int position, string name)>();

            var context = new InstructionContext { Graph = graph };

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var trimmed = line.Trim();

                if (string.IsNullOrEmpty(trimmed) || IsCommentLine(trimmed)) continue;

                var nameMatch = Regex.Match(trimmed, @"^name:\s*(.+)", RegexOptions.IgnoreCase);
                if (nameMatch.Success) continue;

                // Skip function definitions and their 'end' markers
                if (trimmed.StartsWith("function ", StringComparison.OrdinalIgnoreCase) ||
                    trimmed.Equals("end", StringComparison.Ordinal))
                {
                    continue;
                }

                // Handle call lines directly and record their position
                if (trimmed.StartsWith("call ", StringComparison.OrdinalIgnoreCase))
                {
                    var functionName = trimmed.Substring(5).Trim();
                    calls.Add((instructions.Count, functionName));
                    continue;
                }

                // Check if there's a registered handler for this instruction
                var handler = handlerManager.GetHandlerForInstruction(trimmed);
                if (handler != null && handler is IBlockInstructionHandler blockHandler)
                {
                    var res = blockHandler.HandleBlock(lines, ref i, context);
                    if (!res.Success)
                    {
                        SNILDebug.LogError(res.ErrorMessage);
                        continue;
                    }

                    // If the block handler returned a BlockHandlerResult, integrate the instructions and function calls
                    if (res.Data is BlockHandlerResult bhr)
                    {
                        foreach (var fc in bhr.FunctionCalls)
                        {
                            calls.Add((instructions.Count + fc.RelativeInstructionIndex, fc.FunctionName));
                        }

                        instructions.AddRange(bhr.Instructions);
                    }
                    else
                    {
                        // If handler succeeded but returned no data, assume it created nodes directly in the graph and updated context.Nodes
                        SNILDebug.Log("Block handler created nodes directly in the graph.");
                    }

                    continue; // Block handler advances i by reference
                }

                // Fallback: try to match to templates
                var matchedInstruction = MatchLineToTemplate(trimmed, templates);
                if (matchedInstruction != null)
                {
                    instructions.Add(matchedInstruction);
                }
                else
                {
                    SNILDebug.LogWarning($"Unrecognized instruction: {trimmed}");
                }
            }

            return (instructions, calls);
        }

        private static List<SNILInstruction> ParseScript(string[] lines, DialogueGraph graph)
        {
            var (instructions, calls) = ParseLinesToInstructions(lines, graph);
            return instructions;
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

        public static void ApplyInstructionsToGraph(string graphName, List<SNILInstruction> mainInstructions, List<SNILInstruction> functionInstructions, List<int> functionCallPositions, List<string> functionCallNames = null)
        {
            string assetPath = $"Assets/SNEngine/Source/SNEngine/Resources/Dialogues/{graphName}.asset";
            DialogueGraph graph = AssetDatabase.LoadAssetAtPath<DialogueGraph>(assetPath);

            if (graph == null)
            {
                SNILDebug.LogError($"Could not load graph: {assetPath}");
                return;
            }

            SNILNodeCreator.CreateNodesFromInstructions(graph, mainInstructions, functionInstructions, functionCallPositions, functionCallNames);

            EditorUtility.SetDirty(graph);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            SNILDebug.Log($"Imported: {assetPath}");
        }

        private static string SanitizeFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars()) fileName = fileName.Replace(c, '_');
            return string.IsNullOrWhiteSpace(fileName) ? "NewGraph" : fileName;
        }

        private static bool IsCommentLine(string line)
        {
            string trimmed = line.Trim();
            return trimmed.StartsWith("//") || trimmed.StartsWith("#");
        }

        private static SNILInstruction MatchLineToTemplate(string line, Dictionary<string, SNILTemplateInfo> templates)
        {
            foreach (var template in templates)
            {
                var parameters = SNILTemplateMatcher.MatchLineWithTemplate(line, template.Value.Template);
                if (parameters != null)
                {
                    return new SNILInstruction
                    {
                        Type = SNILInstructionType.Generic,
                        NodeTypeName = template.Key,
                        Parameters = parameters,
                        NodeType = SNILTypeResolver.GetNodeType(template.Key)
                    };
                }
            }
            return null;
        }
    }
}