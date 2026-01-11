using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SNEngine.Editor.SNILSystem.Validators
{
    public class NameDirectiveValidator
    {
        public static List<SNILValidationError> ValidateNameDirective(string[] lines)
        {
            var errors = new List<SNILValidationError>();
            
            var mainScriptLines = ScriptLineExtractor.ExtractMainScriptWithoutFunctions(lines);

            // Проверяем наличие директивы name в основном скрипте:
            bool hasNameDirective = false;
            int nameLineIndex = -1;
            for (int i = 0; i < mainScriptLines.Length; i++)
            {
                if (Regex.IsMatch(mainScriptLines[i].Trim(), @"^name:\s*.+", RegexOptions.IgnoreCase))
                {
                    hasNameDirective = true;
                    nameLineIndex = i;
                    break;
                }
            }

            if (!hasNameDirective)
            {
                errors.Add(new SNILValidationError
                {
                    LineNumber = 0,
                    LineContent = "No name directive found",
                    ErrorType = SNILValidationErrorType.MissingNameDirective,
                    Message = "Missing 'name:' directive in script."
                });
            }
            else
            {
                // Проверяем, что первая значимая строка (после name:) - это Start
                // Ищем первую строку после name:
                for (int i = nameLineIndex + 1; i < mainScriptLines.Length; i++)
                {
                    string trimmed = mainScriptLines[i].Trim();
                    if (!string.IsNullOrEmpty(trimmed) && !IsCommentLine(trimmed))
                    {
                        if (!trimmed.Equals("Start", System.StringComparison.OrdinalIgnoreCase))
                        {
                            int originalLineIndex = GetOriginalLineIndex(lines, mainScriptLines[i], 0);
                            errors.Add(new SNILValidationError
                            {
                                LineNumber = originalLineIndex + 1,
                                LineContent = trimmed,
                                ErrorType = SNILValidationErrorType.InvalidStart,
                                Message = "Script must start with 'Start' line after 'name:' directive."
                            });
                        }
                        break; // Проверяем только первую значимую строку
                    }
                }
            }

            return errors;
        }

        private static bool IsCommentLine(string line)
        {
            string trimmed = line.Trim();
            return trimmed.StartsWith("//") || trimmed.StartsWith("#");
        }

        private static int GetOriginalLineIndex(string[] originalLines, string content, int startIndex)
        {
            for (int i = startIndex; i < originalLines.Length; i++)
            {
                if (originalLines[i].Trim() == content.Trim())
                {
                    return i;
                }
            }
            return startIndex; // fallback
        }
    }
}