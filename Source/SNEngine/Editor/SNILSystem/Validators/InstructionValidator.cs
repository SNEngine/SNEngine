using System;
using System.Collections.Generic;
using System.Linq;

namespace SNEngine.Editor.SNILSystem.Validators
{
    public class InstructionValidator
    {
        public static List<SNILValidationError> ValidateInstructions(string[] lines)
        {
            var errors = new List<SNILValidationError>();
            
            var mainScriptLines = ScriptLineExtractor.ExtractMainScriptWithoutFunctions(lines);

            for (int i = 0; i < mainScriptLines.Length; i++)
            {
                string line = mainScriptLines[i];
                string trimmedLine = line.Trim();

                if (string.IsNullOrEmpty(trimmedLine) || IsCommentLine(trimmedLine))
                    continue;

                // Получаем оригинальный индекс строки в исходном массиве
                int originalLineIndex = GetOriginalLineIndex(lines, line, 0);

                // Проверяем специальные инструкции
                if (System.Text.RegularExpressions.Regex.IsMatch(trimmedLine, @"^name:\s*.+", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    // Уже проверили в NameDirectiveValidator
                    continue;
                }
                else if (trimmedLine.Equals("Start", System.StringComparison.OrdinalIgnoreCase))
                {
                    // Уже проверили в NameDirectiveValidator
                    continue;
                }
                else if (trimmedLine.Equals("End", System.StringComparison.OrdinalIgnoreCase))
                {
                    // Allow 'End' inside blocks (e.g., inside If Show Variant branches). Only enforce 'End' to be last
                    // when it's a top-level script terminator.
                    if (!IsLineInsideBlock(mainScriptLines, i) && !IsLastSignificantLine(mainScriptLines, i))
                    {
                        errors.Add(new SNILValidationError
                        {
                            LineNumber = originalLineIndex + 1,
                            LineContent = trimmedLine,
                            ErrorType = SNILValidationErrorType.InvalidEnd,
                            Message = "Script must end with 'End' line."
                        });
                    }
                }
                else if (trimmedLine.StartsWith("Jump To ", System.StringComparison.OrdinalIgnoreCase))
                {
                    // Allow 'Jump To' if it's the last significant line at top-level,
                    // or if it's the last significant line inside the current branch of a block.
                    bool allowed = false;
                    if (!IsLineInsideBlock(mainScriptLines, i))
                    {
                        // top-level Jump To must be last significant line
                        if (IsLastSignificantLine(mainScriptLines, i)) allowed = true;
                    }
                    else
                    {
                        // inside a block: must be the last significant instruction in the branch
                        if (IsLastSignificantInBranch(mainScriptLines, i)) allowed = true;
                    }

                    if (!allowed)
                    {
                        errors.Add(new SNILValidationError
                        {
                            LineNumber = originalLineIndex + 1,
                            LineContent = trimmedLine,
                            ErrorType = SNILValidationErrorType.InvalidEnd,
                            Message = "Script with 'Jump To' must end with this line."
                        });
                    }
                }
                else if (trimmedLine.StartsWith("call ", System.StringComparison.OrdinalIgnoreCase))
                {
                    // Проверяем формат вызова функции
                    string functionName = trimmedLine.Substring(5).Trim();
                    if (string.IsNullOrEmpty(functionName))
                    {
                        errors.Add(new SNILValidationError
                        {
                            LineNumber = originalLineIndex + 1,
                            LineContent = trimmedLine,
                            ErrorType = SNILValidationErrorType.InvalidFunctionDefinition,
                            Message = "Function name is required after 'call' keyword."
                        });
                    }
                }
                else if (trimmedLine.StartsWith("function ", System.StringComparison.OrdinalIgnoreCase))
                {
                    // Уже обработали в FunctionValidator
                    continue;
                }
                else if (trimmedLine.Equals("end", System.StringComparison.Ordinal))
                {
                    // Уже обработали в FunctionValidator
                    continue;
                }
                else
                {
                    // Для всех остальных инструкций используем систему валидаторов
                    var validationResult = InstructionValidatorManager.Instance.ValidateInstruction(trimmedLine);
                    if (!validationResult.IsValid)
                    {
                        errors.Add(new SNILValidationError
                        {
                            LineNumber = originalLineIndex + 1,
                            LineContent = trimmedLine,
                            ErrorType = SNILValidationErrorType.UnknownNode,
                            Message = validationResult.ErrorMessage
                        });
                    }
                }
            }

            // Ensure the top-level script ends with an End or Jump To (End inside blocks is allowed)
            int lastSignificantIdx = -1;
            for (int i = mainScriptLines.Length - 1; i >= 0; i--)
            {
                var t = mainScriptLines[i].Trim();
                if (!string.IsNullOrEmpty(t) && !IsCommentLine(t))
                {
                    lastSignificantIdx = i;
                    break;
                }
            }

            if (lastSignificantIdx == -1)
            {
                errors.Add(new SNILValidationError
                {
                    LineNumber = 0,
                    LineContent = "",
                    ErrorType = SNILValidationErrorType.NoContent,
                    Message = "Script contains no content."
                });
            }
            else
            {
                var lastTrim = mainScriptLines[lastSignificantIdx].Trim();
                bool ok = false;
                if (lastTrim.StartsWith("Jump To ", StringComparison.OrdinalIgnoreCase)) ok = true;
                if (lastTrim.Equals("End", StringComparison.OrdinalIgnoreCase) && !IsLineInsideBlock(mainScriptLines, lastSignificantIdx)) ok = true;

                if (!ok)
                {
                    // If the script ends with a top-level 'If Show Variant' block that itself guarantees termination
                    // (all its branches end with 'End' or 'Jump To'), consider it valid.
                    if (lastTrim.Equals("endif", StringComparison.OrdinalIgnoreCase))
                    {
                        int ifStartIdx = FindMatchingIfStart(mainScriptLines, lastSignificantIdx);
                        if (ifStartIdx >= 0 && IsIfBlockTerminating(mainScriptLines, ifStartIdx, lastSignificantIdx))
                        {
                            ok = true;
                        }
                    }
                    // If the script ends with a top-level 'Switch Show Variant' block that itself guarantees termination
                    // (all its cases end with 'End' or 'Jump To'), consider it valid.
                    else if (lastTrim.Equals("endcase", StringComparison.OrdinalIgnoreCase))
                    {
                        int switchStartIdx = FindMatchingSwitchStart(mainScriptLines, lastSignificantIdx);
                        if (switchStartIdx >= 0 && IsSwitchBlockTerminating(mainScriptLines, switchStartIdx, lastSignificantIdx))
                        {
                            ok = true;
                        }
                    }
                }

                if (!ok)
                {
                    int originalLineIndex = GetOriginalLineIndex(lines, mainScriptLines[lastSignificantIdx], 0);
                    errors.Add(new SNILValidationError
                    {
                        LineNumber = originalLineIndex + 1,
                        LineContent = mainScriptLines[lastSignificantIdx].Trim(),
                        ErrorType = SNILValidationErrorType.InvalidEnd,
                        Message = "Script must end with a top-level 'End' or 'Jump To' instruction."
                    });
                }
            }

            return errors;
        }

        private static bool IsLastSignificantLine(string[] lines, int currentIndex)
        {
            for (int i = currentIndex + 1; i < lines.Length; i++)
            {
                string trimmed = lines[i].Trim();
                if (!string.IsNullOrEmpty(trimmed) && !IsCommentLine(trimmed))
                {
                    return false; // Найдена еще одна значимая строка
                }
            }
            return true; // Это последняя значимая строка
        }

        // Проверяет, является ли текущая строка последней значимой строкой внутри своей ветки
        private static bool IsLastSignificantInBranch(string[] lines, int index)
        {
            int nesting = 0;
            for (int i = index + 1; i < lines.Length; i++)
            {
                var t = lines[i].Trim();
                if (string.IsNullOrEmpty(t) || IsCommentLine(t)) continue;
                if (t.Equals("If Show Variant", StringComparison.OrdinalIgnoreCase)) { nesting++; continue; }
                if (t.Equals("Switch Show Variant", StringComparison.OrdinalIgnoreCase)) { nesting++; continue; }
                if (t.Equals("endif", StringComparison.OrdinalIgnoreCase))
                {
                    if (nesting == 0) return true; // дошли до конца внешнего блока
                    nesting--;
                    continue;
                }
                if (t.Equals("endcase", StringComparison.OrdinalIgnoreCase))
                {
                    if (nesting == 0) return true; // дошли до конца внешнего блока
                    nesting--;
                    continue;
                }
                if ((t.EndsWith(":") || t.StartsWith("Case ")) && nesting == 0)
                {
                    // следующий заголовок секции на том же уровне — ветка закончилась
                    return true;
                }
                if (nesting == 0 && !t.EndsWith(":") && !t.StartsWith("Case ")) return false; // найдена значимая инструкция после
            }

            // если файл кончился, то это конец ветки
            return true;
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

        private static bool IsLineInsideBlock(string[] lines, int index)
        {
            // Simple block stack: increment on encountering a block start (e.g. "If Show Variant"), decrement on "endif"
            int open = 0;
            for (int i = 0; i <= index; i++)
            {
                var t = lines[i].Trim();
                if (t.Equals("If Show Variant", StringComparison.OrdinalIgnoreCase)) open++;
                if (t.Equals("Switch Show Variant", StringComparison.OrdinalIgnoreCase)) open++;
                if (t.Equals("endif", StringComparison.OrdinalIgnoreCase) && open > 0) open--;
                if (t.Equals("endcase", StringComparison.OrdinalIgnoreCase) && open > 0) open--;
            }
            return open > 0;
        }

        private static int FindMatchingIfStart(string[] lines, int endifIndex)
        {
            int depth = 0;
            for (int i = endifIndex; i >= 0; i--)
            {
                var t = lines[i].Trim();
                if (t.Equals("endif", StringComparison.OrdinalIgnoreCase))
                {
                    depth++;
                }
                else if (t.Equals("If Show Variant", StringComparison.OrdinalIgnoreCase))
                {
                    depth--;
                    if (depth == 0) return i;
                }
            }

            return -1; // not found
        }

        private static bool IsIfBlockTerminating(string[] lines, int ifStart, int endifIndex)
        {
            // We consider the block terminating if every branch present (True/False or variant-named sections) ends with 'End' or 'Jump To'
            var branches = new List<(int start, int end)>();
            int i = ifStart + 1;
            // Skip Variants: section
            while (i < endifIndex && (string.IsNullOrWhiteSpace(lines[i]) || lines[i].TrimStart().StartsWith("//") || lines[i].TrimStart().StartsWith("#"))) i++;
            if (i < endifIndex && lines[i].Trim().StartsWith("Variants", StringComparison.OrdinalIgnoreCase))
            {
                i++; // skip header
                while (i < endifIndex)
                {
                    var t = lines[i].Trim();
                    if (string.IsNullOrEmpty(t) || t.StartsWith("//") || t.StartsWith("#")) { i++; continue; }
                    if (t.EndsWith(":")) break; // next section header
                    i++;
                }
            }

            // Now collect sections until endif
            while (i < endifIndex)
            {
                var t = lines[i].Trim();
                if (string.IsNullOrEmpty(t) || t.StartsWith("//") || t.StartsWith("#")) { i++; continue; }
                if (t.EndsWith(":"))
                {
                    int sectionStart = i + 1;
                    var header = t.Substring(0, t.Length - 1).Trim();
                    // collect until next header or endif
                    int j = sectionStart;
                    int nestedIf = 0;
                    int lastSignificant = -1;
                    while (j < endifIndex)
                    {
                        var line = lines[j].Trim();
                        if (line.Equals("If Show Variant", StringComparison.OrdinalIgnoreCase)) { nestedIf++; }
                        else if (line.Equals("endif", StringComparison.OrdinalIgnoreCase))
                        {
                            if (nestedIf > 0) { nestedIf--; }
                            else break; // this would be handled by outer loop
                        }

                        if (nestedIf == 0 && !string.IsNullOrEmpty(line) && !line.StartsWith("//") && !line.StartsWith("#") && !line.EndsWith(":"))
                        {
                            lastSignificant = j;
                        }

                        // stop when we see next section header at nesting 0
                        if (nestedIf == 0 && j + 1 < endifIndex && lines[j + 1].Trim().EndsWith(":")) { j++; break; }

                        j++;
                    }

                    if (lastSignificant == -1)
                    {
                        // empty branch -> not terminating
                        return false;
                    }

                    var last = lines[lastSignificant].Trim();
                    if (!(last.Equals("End", StringComparison.OrdinalIgnoreCase) || last.StartsWith("Jump To ", StringComparison.OrdinalIgnoreCase)))
                    {
                        return false; // branch doesn't end properly
                    }

                    i = j + 1;
                    continue;
                }

                // Unexpected lines between sections - skip
                i++;
            }

            // all branches checked
            return true;
        }

        private static int FindMatchingSwitchStart(string[] lines, int endcaseIndex)
        {
            int depth = 0;
            for (int i = endcaseIndex; i >= 0; i--)
            {
                var t = lines[i].Trim();
                if (t.Equals("endcase", StringComparison.OrdinalIgnoreCase))
                {
                    depth++;
                }
                else if (t.Equals("Switch Show Variant", StringComparison.OrdinalIgnoreCase))
                {
                    depth--;
                    if (depth == 0) return i;
                }
            }

            return -1; // not found
        }

        private static bool IsSwitchBlockTerminating(string[] lines, int switchStart, int endcaseIndex)
        {
            // We consider the block terminating if every case present ends with 'End' or 'Jump To'
            int i = switchStart + 1;
            // Skip Cases: section
            while (i < endcaseIndex && (string.IsNullOrWhiteSpace(lines[i]) || lines[i].TrimStart().StartsWith("//") || lines[i].TrimStart().StartsWith("#"))) i++;
            if (i < endcaseIndex && lines[i].Trim().Equals("Cases:", StringComparison.OrdinalIgnoreCase))
            {
                i++; // skip header
                while (i < endcaseIndex)
                {
                    var t = lines[i].Trim();
                    if (string.IsNullOrEmpty(t) || t.StartsWith("//") || t.StartsWith("#")) { i++; continue; }
                    if (t.StartsWith("Case ", StringComparison.OrdinalIgnoreCase)) break; // next case header
                    i++;
                }
            }

            // Now collect case sections until endcase
            while (i < endcaseIndex)
            {
                var t = lines[i].Trim();
                if (string.IsNullOrEmpty(t) || t.StartsWith("//") || t.StartsWith("#")) { i++; continue; }
                if (t.StartsWith("Case ", StringComparison.OrdinalIgnoreCase))
                {
                    int sectionStart = i + 1;
                    // collect until next header or endcase
                    int j = sectionStart;
                    int nestedSwitch = 0;
                    int lastSignificant = -1;
                    while (j < endcaseIndex)
                    {
                        var line = lines[j].Trim();
                        if (line.Equals("Switch Show Variant", StringComparison.OrdinalIgnoreCase)) { nestedSwitch++; }
                        else if (line.Equals("endcase", StringComparison.OrdinalIgnoreCase))
                        {
                            if (nestedSwitch > 0) { nestedSwitch--; }
                            else break; // this would be handled by outer loop
                        }

                        if (nestedSwitch == 0 && !string.IsNullOrEmpty(line) && !line.StartsWith("//") && !line.StartsWith("#") && !line.StartsWith("Case ", StringComparison.OrdinalIgnoreCase))
                        {
                            lastSignificant = j;
                        }

                        // stop when we see next case header at nesting 0
                        if (nestedSwitch == 0 && j + 1 < endcaseIndex && lines[j + 1].Trim().StartsWith("Case ", StringComparison.OrdinalIgnoreCase)) { j++; break; }

                        j++;
                    }

                    if (lastSignificant == -1)
                    {
                        // empty case -> not terminating
                        return false;
                    }

                    var last = lines[lastSignificant].Trim();
                    if (!(last.Equals("End", StringComparison.OrdinalIgnoreCase) || last.StartsWith("Jump To ", StringComparison.OrdinalIgnoreCase)))
                    {
                        return false; // case doesn't end properly
                    }

                    i = j + 1;
                    continue;
                }

                // Unexpected lines between sections - skip
                i++;
            }

            // all cases checked
            return true;
        }
    }
}