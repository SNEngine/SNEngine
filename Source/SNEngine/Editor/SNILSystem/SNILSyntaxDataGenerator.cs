using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text;
using SNEngine.IO;
using SNEngine.Editor.SNILSystem;

namespace SNEngine.Editor.SNILSystem
{
    public class SNILSyntaxDataGenerator
    {
        private static string BasePath => NovelFile.DataPath;
        private static string TemplatesRelativePath = "SNEngine/Source/SNEngine/Editor/SNIL";

        [MenuItem("SNEngine/SNIL/Generate SNIL Syntax")]
        public static void GenerateWithDialog()
        {
            string fullTemplatesPath = Path.Combine(BasePath, TemplatesRelativePath);

            if (!NovelDirectory.Exists(fullTemplatesPath))
            {
                SNILDebug.LogError($"Template directory not found at: {fullTemplatesPath}");
                return;
            }

            string selectedPath = EditorUtility.SaveFolderPanel("Save Syntax JSON", BasePath, "snil_syntax.json");

            if (string.IsNullOrEmpty(selectedPath))
            {
                return;
            }

            string finalFilePath = Path.Combine(selectedPath, "snil_syntax.json");
            List<string> jsonEntries = new List<string>();

            string[] workerFiles = NovelDirectory.GetFiles(fullTemplatesPath, "*.snil", SearchOption.TopDirectoryOnly);

            foreach (string filePath in workerFiles)
            {
                string content = NovelFile.ReadAllText(filePath);
                string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                if (lines.Length == 0) continue;

                string firstLine = lines[0].Trim();
                string workerName = ExtractWorkerNameFromContent(lines);

                if (!string.IsNullOrEmpty(workerName))
                {
                    string escapedFirstLine = firstLine.Replace("\\", "\\\\").Replace("\"", "\\\"");
                    jsonEntries.Add($"\"{workerName}\": \"{escapedFirstLine}\"");
                }
            }

            string jsonOutput = "{\n  " + string.Join(",\n  ", jsonEntries) + "\n}";

            NovelFile.WriteAllText(finalFilePath, jsonOutput, new UTF8Encoding(false));

            AssetDatabase.Refresh();
            SNILDebug.Log($"Syntax data saved for editors: {finalFilePath}");
        }

        private static string ExtractWorkerNameFromContent(string[] lines)
        {
            foreach (var line in lines)
            {
                string trimmed = line.Trim();
                if (trimmed.StartsWith("worker:"))
                {
                    return trimmed.Replace("worker:", "").Trim();
                }
            }
            return null;
        }
    }
}