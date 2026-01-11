using System;
using System.Collections.Generic;
using SNEngine.Editor.SNILSystem.FunctionSystem;
using SNEngine.Editor.SNILSystem.Importers;
using SNEngine.Editor.SNILSystem.InstructionHandlers;
using SNEngine.Editor.SNILSystem.Parsers;
using SNEngine.Editor.SNILSystem.Validators;
using SNEngine.Graphs;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem
{
    public class SNILCompiler
    {
        public static bool ImportScript(string filePath)
        {
            // Используем новую систему обработчиков инструкций
            return SNILInstructionBasedCompiler.CompileScript(filePath);
        }

        public static bool ValidateScript(string filePath, out List<SNILValidationError> errors)
        {
            return SNILScriptValidator.ValidateScript(filePath, out errors);
        }

        public static bool ImportScriptWithoutPostProcessing(string filePath)
        {
            return SNILInstructionBasedCompiler.CompileScriptWithoutPostProcessing(filePath);
        }

        public static List<string> GetAllGraphNamesInFile(string filePath)
        {
            return SNILScriptValidator.GetAllGraphNamesInFile(filePath);
        }

        public static bool CreateAllGraphsInFile(string filePath)
        {
            try
            {
                SNILScriptValidator.CreateAllGraphsInFile(filePath);
                return true;
            }
            catch (System.Exception ex)
            {
                SNILDebug.LogError($"Create all graphs failed with exception: {ex.Message}");
                return false;
            }
        }

        public static bool ProcessAllGraphsInFile(string filePath)
        {
            var scriptParts = SNILMultiScriptParser.ParseMultiScript(filePath);
            bool allSuccessful = true;

            foreach (string[] part in scriptParts)
            {
                // Используем новую систему обработчиков инструкций
                if (!ProcessSingleScriptPart(part))
                {
                    allSuccessful = false;
                }
            }

            return allSuccessful;
        }

        private static bool ProcessSingleScriptPart(string[] lines)
        {
            // Используем ту же логику, что и в CompileSingleScript
            if (lines.Length == 0) return true;

            // Валидация
            Validators.SNILSyntaxValidator validator = new Validators.SNILSyntaxValidator();
            if (!validator.Validate(lines, out string errorMessage))
            {
                SNILDebug.LogError($"SNIL script validation failed: {errorMessage}");
                return false;
            }

            // Извлекаем функции и основной скрипт один раз
            var functions = SNILFunctionParser.ParseFunctions(lines);
            var mainScriptLines = SNILFunctionParser.ExtractMainScriptWithoutFunctions(lines).ToArray();

            // Создаем контекст выполнения
            var context = new InstructionContext();

            // Регистрируем все функции в контексте
            foreach (var function in functions)
            {
                if (!context.Functions.ContainsKey(function.Name))
                    context.Functions.Add(function.Name, function);
                else
                    context.Functions[function.Name] = function;
            }

            bool hasProcessingErrors = false;
            List<string> errorMessages = new List<string>();

            // Обрабатываем основной скрипт (после регистрации функций)
            foreach (string line in mainScriptLines)
            {
                string trimmedLine = line.Trim();

                if (string.IsNullOrEmpty(trimmedLine) || IsCommentLine(trimmedLine))
                    continue;

                // Используем менеджер обработчиков для обработки инструкции
                var result = InstructionHandlerManager.Instance.ProcessInstruction(trimmedLine, context);

                if (!result.Success)
                {
                    string errorMsg = $"Failed to process instruction '{trimmedLine}': {result.ErrorMessage}";
                    SNILDebug.LogError(errorMsg);
                    errorMessages.Add(errorMsg);
                    hasProcessingErrors = true;
                }
            }

            // Если были ошибки обработки инструкций, не продолжаем импорт
            if (hasProcessingErrors)
            {
                SNILDebug.LogError($"Script processing failed with the following errors:\n{string.Join("\n", errorMessages)}");
                return false;
            }

            // После обработки всех инструкций, соединяем ноды последовательно
            if (context.Graph != null)
            {
                var dialogueGraph = (DialogueGraph)context.Graph;
                NodeConnectionUtility.ConnectNodesSequentially(dialogueGraph, context.Nodes);
            }

            return true;
        }

        private static bool IsCommentLine(string line)
        {
            string trimmed = line.Trim();
            return trimmed.StartsWith("//") || trimmed.StartsWith("#");
        }
    }

    public class SNILInstruction
    {
        public SNILInstructionType Type { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public string NodeTypeName { get; set; }
        public Type NodeType { get; set; }
    }

    public enum SNILInstructionType { Generic }
}