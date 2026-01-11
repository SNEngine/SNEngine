using UnityEditor;
using System.IO;
using System.Text;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using SNEngine.IO;
using UnityEngine;
using SNEngine.Editor.SNILSystem;

namespace SNEngine.Editor.SNIL
{
    public class SNILDocumentationGenerator
    {
        private static string BasePath => NovelFile.DataPath;
        private static string TemplatesRelativePath = "SNEngine/Source/SNEngine/Editor/SNIL";
        private static string ManualRelativePath = "SNEngine/Source/SNEngine/Editor/SNIL/Manual";
        private static string ReadmeRelativePath = "SNEngine/Source/SNEngine/Editor/SNILSystem/README_Workers.md";

        [MenuItem("SNEngine/SNIL/Generate SNIL Documentation")]
        public static void Generate()
        {
            string fullTemplatesPath = Path.Combine(BasePath, TemplatesRelativePath);
            string fullManualPath = Path.Combine(BasePath, ManualRelativePath);
            string fullReadmePath = Path.Combine(BasePath, ReadmeRelativePath);

            if (!NovelDirectory.Exists(fullTemplatesPath))
            {
                SNILDebug.LogError($"Template directory not found at: {fullTemplatesPath}");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# List Commands");
            sb.AppendLine();

            string[] workerFiles = NovelDirectory.GetFiles(fullTemplatesPath, "*.snil", SearchOption.TopDirectoryOnly);
            foreach (string filePath in workerFiles)
            {
                ProcessSnilFile(filePath, sb, true);
            }
            if (NovelDirectory.Exists(fullManualPath))
            {
                string[] manualFiles = NovelDirectory.GetFiles(fullManualPath, "*.snil", SearchOption.AllDirectories);
                foreach (string filePath in manualFiles)
                {
                    ProcessSnilFile(filePath, sb, false);
                }
            }

            NovelFile.WriteAllText(fullReadmePath, sb.ToString());
            AssetDatabase.Refresh();

            SNILDebug.Log($"Documentation successfully generated at: {fullReadmePath}");
        }

        private static void ProcessSnilFile(string filePath, StringBuilder sb, bool parseWorkerName)
        {
            string content = NovelFile.ReadAllText(filePath);
            string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            string displayName = Path.GetFileNameWithoutExtension(filePath);
            var syntaxLines = new System.Collections.Generic.List<string>();

            foreach (var line in lines)
            {
                if (parseWorkerName && line.Trim().StartsWith("worker:"))
                {
                    string rawName = line.Replace("worker:", "").Trim();
                    displayName = FormatWorkerName(rawName);
                    continue;
                }
                syntaxLines.Add(line);
            }

            sb.AppendLine($"### {displayName}");
            sb.AppendLine("```snil");
            foreach (string syntax in syntaxLines)
            {
                sb.AppendLine(syntax);
            }
            sb.AppendLine("```");
            sb.AppendLine();
        }

        private static string FormatWorkerName(string workerName)
        {
            string nameWithoutSuffix = workerName.Replace("NodeWorker", "").Replace("Worker", "");
            return Regex.Replace(nameWithoutSuffix, "([a-z])([A-Z])", "$1 $2").Trim();
        }
    }
}