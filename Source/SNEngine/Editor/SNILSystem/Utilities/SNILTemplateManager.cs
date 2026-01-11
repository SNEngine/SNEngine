using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SNEngine.Editor.SNILSystem
{
    public class SNILTemplateManager
    {
        private static Dictionary<string, SNILTemplateInfo> _nodeTemplates;

        static SNILTemplateManager()
        {
            LoadNodeTemplates();
        }

        private static void LoadNodeTemplates()
        {
            _nodeTemplates = new Dictionary<string, SNILTemplateInfo>();

            string snilDirectory = "Assets/SNEngine/Source/SNEngine/Editor/SNIL";
            if (Directory.Exists(snilDirectory))
            {
                string[] templateFiles = Directory.GetFiles(snilDirectory, "*.snil");

                foreach (string templateFile in templateFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(templateFile);
                    if (fileName.EndsWith(".cs")) fileName = Path.GetFileNameWithoutExtension(fileName);

                    string[] lines = File.ReadAllLines(templateFile);
                    string templateContent = "";
                    string workerName = null;

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("worker:", System.StringComparison.OrdinalIgnoreCase))
                        {
                            workerName = line.Substring(7).Trim();
                        }
                        else if (!string.IsNullOrEmpty(line) && !line.StartsWith("worker:", System.StringComparison.OrdinalIgnoreCase))
                        {
                            templateContent = line; // Берём первую непустую строку как шаблон
                            break;
                        }
                    }

                    _nodeTemplates[fileName] = new SNILTemplateInfo
                    {
                        Template = templateContent,
                        WorkerName = workerName
                    };
                }
            }
        }

        public static Dictionary<string, SNILTemplateInfo> GetNodeTemplates()
        {
            return _nodeTemplates;
        }
        
        public static SNILTemplateInfo GetTemplateInfo(string nodeName)
        {
            return _nodeTemplates.ContainsKey(nodeName) ? _nodeTemplates[nodeName] : null;
        }

        public static void ReloadTemplates()
        {
            LoadNodeTemplates();
        }
    }
    
    public class SNILTemplateInfo
    {
        public string Template { get; set; }
        public string WorkerName { get; set; }
    }
}