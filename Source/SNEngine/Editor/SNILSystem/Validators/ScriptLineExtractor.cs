using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SNEngine.Editor.SNILSystem.Validators
{
    public static class ScriptLineExtractor
    {
        public static string[] ExtractMainScriptWithoutFunctions(string[] lines)
        {
            List<string> mainScript = new List<string>();
            bool insideFunction = false;

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("function ", System.StringComparison.OrdinalIgnoreCase))
                {
                    insideFunction = true;
                    // Do NOT add function definition to main script
                }
                // Only consider lowercase "end" as function end, not uppercase "End"
                else if (trimmedLine.Equals("end", System.StringComparison.Ordinal) && insideFunction)
                {
                    insideFunction = false;
                    // Do NOT add function end to main script
                }
                else if (!insideFunction)
                {
                    // Add line to main script only if we are not inside a function
                    mainScript.Add(line);
                }
                // Lines inside functions are ignored when extracting main script
            }

            return mainScript.ToArray();
        }
    }
}