using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using SNEngine.Editor.SNILSystem.FunctionSystem;
using SNEngine.Editor.SNILSystem.InstructionHandlers;
using SNEngine.Editor.SNILSystem.Parsers;
using SNEngine.Editor.SNILSystem.Validators;
using SNEngine.Graphs;
using UnityEngine;

namespace SNEngine.Editor.SNILSystem
{
    public class SNILInstructionBasedCompiler
    {
        public static bool CompileScript(string filePath)
        {
            return CompileScriptInternal(filePath, true);
        }

        public static bool CompileScriptWithoutPostProcessing(string filePath)
        {
            return CompileScriptInternal(filePath, false);
        }

        private static bool CompileScriptInternal(string filePath, bool doPostProcessing)
        {
            try
            {
                filePath = filePath.Trim().Trim('"', '@', '\'');

                if (!File.Exists(filePath))
                {
                    SNILDebug.LogError($"File not found: {filePath}");
                    return false;
                }

                // Reload templates to ensure latest changes are used
                SNILTemplateManager.ReloadTemplates();

                List<string[]> scriptParts = SNILMultiScriptParser.ParseMultiScript(filePath);

                bool allSuccessful = true;
                if (scriptParts.Count > 1)
                {
                    allSuccessful = CompileMultiScript(scriptParts);
                }
                else
                {
                    allSuccessful = CompileSingleScript(scriptParts[0]);
                }

                if (allSuccessful && doPostProcessing)
                {
                    SNILPostProcessor.ProcessAllReferences();
                }

                return allSuccessful;
            }
            catch (Exception e)
            {
                SNILDebug.LogError($"Compilation failed: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        private static bool CompileMultiScript(List<string[]> scriptParts)
        {
            bool allSuccessful = true;
            foreach (string[] part in scriptParts)
            {
                if (!CompileSingleScript(part))
                {
                    allSuccessful = false;
                }
            }
            return allSuccessful;
        }

        private static bool CompileSingleScript(string[] lines)
        {
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
            for (int i = 0; i < mainScriptLines.Length; i++)
            {
                string line = mainScriptLines[i];
                string trimmedLine = line.Trim();

                if (string.IsNullOrEmpty(trimmedLine) || IsCommentLine(trimmedLine))
                    continue;

                // Проверяем, является ли инструкция блочной (например, If Show Variant)
                var handler = InstructionHandlerManager.Instance.GetHandlerForInstruction(trimmedLine);

                // Check if it's a block instruction handler that can handle block instructions
                if (handler is IBlockInstructionHandler blockHandler)
                {
                    // Process the entire block using the special method
                    var result = ((IBlockInstructionHandler)handler).HandleBlock(mainScriptLines, ref i, context);
                    if (!result.Success)
                    {
                        string errorMsg = $"Failed to process block instruction '{trimmedLine}': {result.ErrorMessage}";
                        SNILDebug.LogError(errorMsg);
                        errorMessages.Add(errorMsg);
                        hasProcessingErrors = true;
                    }
                }
                else
                {
                    // Используем менеджер обработчиков для обработки обычной инструкции
                    var result = InstructionHandlerManager.Instance.ProcessInstruction(trimmedLine, context);

                    if (!result.Success)
                    {
                        string errorMsg = $"Failed to process instruction '{trimmedLine}': {result.ErrorMessage}";
                        SNILDebug.LogError(errorMsg);
                        errorMessages.Add(errorMsg);
                        hasProcessingErrors = true;
                    }
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
}