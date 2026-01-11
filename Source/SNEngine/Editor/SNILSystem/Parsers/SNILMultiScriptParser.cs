using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SNEngine.Editor.SNILSystem.Parsers
{
    public class SNILMultiScriptParser
    {
        public static List<string[]> ParseMultiScript(string filePath)
        {
            List<string[]> scripts = new List<string[]>();
            
            string[] allLines = File.ReadAllLines(filePath);
            List<string> currentScriptLines = new List<string>();
            
            foreach (string line in allLines)
            {
                if (IsScriptSeparator(line))
                {
                    // Если текущий скрипт не пустой, добавляем его
                    if (currentScriptLines.Count > 0)
                    {
                        scripts.Add(currentScriptLines.ToArray());
                        currentScriptLines = new List<string>();
                    }
                }
                else
                {
                    currentScriptLines.Add(line);
                }
            }
            
            // Добавляем последний скрипт, если он не пустой
            if (currentScriptLines.Count > 0)
            {
                scripts.Add(currentScriptLines.ToArray());
            }
            
            return scripts;
        }
        
        private static bool IsScriptSeparator(string line)
        {
            string trimmed = line.Trim();
            return trimmed.Equals("---") || 
                   trimmed.Equals("***") || 
                   trimmed.StartsWith("---") || 
                   trimmed.StartsWith("***");
        }
    }
}