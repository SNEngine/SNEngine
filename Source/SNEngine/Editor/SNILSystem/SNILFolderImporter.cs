using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace SNEngine.Editor.SNILSystem
{
    public static class SNILFolderImporter
    {
        [MenuItem("SNEngine/SNIL/Import SNIL Folder", priority = 102)]
        public static void ImportSNILFolder()
        {
            string selectedPath = EditorUtility.OpenFolderPanel(
                "Select SNIL Scripts Folder",
                "",
                ""
            );

            if (!string.IsNullOrEmpty(selectedPath))
            {
                ImportFolder(selectedPath);
            }
        }

        public static void ImportFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                SNILDebug.LogError($"Folder does not exist: {folderPath}");
                return;
            }

            string[] snilFiles = Directory.GetFiles(folderPath, "*.snil", SearchOption.AllDirectories);

            if (snilFiles.Length == 0)
            {
                SNILDebug.LogWarning($"No .snil files found in folder: {folderPath}");
                return;
            }

            // Сначала проверяем валидность всех файлов
            SNILDebug.Log("Validating all files...");
            bool allValid = true;
            foreach (string filePath in snilFiles)
            {
                if (!ValidateFile(filePath, out List<Validators.SNILValidationError> fileErrors))
                {
                    allValid = false;
                    string errorDetails = string.Join("\n  ", fileErrors.ConvertAll(e => e.ToString()));
                    SNILDebug.LogError($"Validation failed for file: {filePath}\n  {errorDetails}");
                }
            }

            if (!allValid)
            {
                SNILDebug.LogError("One or more files failed validation. Import cancelled.");
                return;
            }

            // Если все файлы прошли валидацию, начинаем создание графов
            SNILDebug.Log("All files validated successfully. Creating all graphs...");
            foreach (string filePath in snilFiles)
            {
                SNILCompiler.CreateAllGraphsInFile(filePath);
            }

            // Затем добавляем ноды ко всем графам
            SNILDebug.Log("Adding nodes to all graphs...");
            foreach (string filePath in snilFiles)
            {
                SNILCompiler.ProcessAllGraphsInFile(filePath);
            }

            // И только потом обрабатываем все ссылки
            SNILDebug.Log("Processing all references...");
            SNILPostProcessor.ProcessAllReferences();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            SNILDebug.Log($"Successfully imported {snilFiles.Length} .snil files from {folderPath}");
        }

        private static bool ValidateFile(string filePath, out List<Validators.SNILValidationError> errors)
        {
            errors = new List<Validators.SNILValidationError>();
            try
            {
                string[] lines = System.IO.File.ReadAllLines(filePath);
                var scriptParts = Parsers.SNILMultiScriptParser.ParseMultiScript(filePath);
                
                bool isValid = true;
                int partIndex = 0;
                
                foreach (string[] part in scriptParts)
                {
                    Validators.SNILSyntaxValidator validator = new Validators.SNILSyntaxValidator();
                    if (!validator.Validate(part, out string errorMessage, out List<Validators.SNILValidationError> partErrors))
                    {
                        // Добавляем номер части к номерам строк для лучшей идентификации
                        string partName = GetPartName(part);
                        foreach (var error in partErrors)
                        {
                            error.Message = $"[Part: {partName}] {error.Message}";
                            errors.Add(error);
                        }
                        isValid = false;
                    }
                    partIndex++;
                }
                
                return isValid;
            }
            catch (System.Exception e)
            {
                SNILDebug.LogError($"Error validating file {filePath}: {e.Message}");
                return false;
            }
        }

        private static string GetPartName(string[] part)
        {
            // Ищем имя части в строке name:
            foreach (string line in part)
            {
                var nameMatch = System.Text.RegularExpressions.Regex.Match(line.Trim(), @"^name:\s*(.+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (nameMatch.Success)
                {
                    return nameMatch.Groups[1].Value.Trim();
                }
            }
            return "Unknown";
        }
    }
}