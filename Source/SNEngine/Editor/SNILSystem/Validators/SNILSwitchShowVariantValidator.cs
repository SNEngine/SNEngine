using System;
using System.Collections.Generic;
using System.Linq;

namespace SNEngine.Editor.SNILSystem.Validators
{
    public static class SNILSwitchShowVariantValidator
    {
        public static List<SNILValidationError> Validate(string[] lines)
        {
            var errors = new List<SNILValidationError>();

            for (int i = 0; i < lines.Length; i++)
            {
                var t = lines[i].Trim();
                if (string.IsNullOrEmpty(t) || t.StartsWith("//") || t.StartsWith("#")) continue;

                if (t.Equals("Switch Show Variant", StringComparison.OrdinalIgnoreCase))
                {
                    int start = i;
                    int j = i + 1;

                    // Find Cases: header
                    while (j < lines.Length && (string.IsNullOrWhiteSpace(lines[j]) || lines[j].TrimStart().StartsWith("//") || lines[j].TrimStart().StartsWith("#"))) j++;
                    if (j >= lines.Length || !lines[j].Trim().Equals("Cases:", StringComparison.OrdinalIgnoreCase))
                    {
                        errors.Add(new SNILValidationError
                        {
                            LineNumber = start + 1,
                            LineContent = lines[start],
                            ErrorType = SNILValidationErrorType.SwitchMissingCases,
                            Message = "'Switch Show Variant' block must contain a 'Cases:' section."
                        });

                        // Try to continue searching (skip ahead)
                        i = j;
                        continue;
                    }

                    // Skip the 'Cases:' header
                    j++;

                    // Now parse sections until matching endcase (support nesting)
                    int nest = 0;
                    bool foundCase = false;
                    int scan = j;
                    while (scan < lines.Length)
                    {
                        var line = lines[scan].Trim();
                        if (string.IsNullOrEmpty(line) || line.StartsWith("//") || line.StartsWith("#")) { scan++; continue; }
                        if (line.Equals("Switch Show Variant", StringComparison.OrdinalIgnoreCase)) { nest++; scan++; continue; }
                        if (line.Equals("If Show Variant", StringComparison.OrdinalIgnoreCase)) { nest++; scan++; continue; }
                        if (line.Equals("endcase", StringComparison.OrdinalIgnoreCase))
                        {
                            if (nest == 0)
                            {
                                // Found block end
                                break;
                            }
                            nest--;
                            scan++;
                            continue;
                        }
                        if (line.Equals("endif", StringComparison.OrdinalIgnoreCase))
                        {
                            if (nest == 0)
                            {
                                errors.Add(new SNILValidationError
                                {
                                    LineNumber = scan + 1,
                                    LineContent = lines[scan],
                                    ErrorType = SNILValidationErrorType.SwitchMissingEnd,
                                    Message = "'Switch Show Variant' block is not closed with 'endcase'."
                                });
                                break;
                            }
                            nest--;
                            scan++;
                            continue;
                        }

                        if (line.StartsWith("Case ", StringComparison.OrdinalIgnoreCase))
                        {
                            foundCase = true;
                            // case header
                            int caseHeaderLine = scan;
                            // find last significant line inside case
                            int k = scan + 1;
                            int nestedSwitch = 0;
                            int lastSignificant = -1;
                            while (k < lines.Length)
                            {
                                var ln = lines[k].Trim();
                                if (string.IsNullOrEmpty(ln) || ln.StartsWith("//") || ln.StartsWith("#")) { k++; continue; }
                                if (ln.StartsWith("Case ", StringComparison.OrdinalIgnoreCase) && nestedSwitch == 0) break; // next case
                                if (ln.Equals("Switch Show Variant", StringComparison.OrdinalIgnoreCase)) { nestedSwitch++; }
                                else if (ln.Equals("If Show Variant", StringComparison.OrdinalIgnoreCase)) { nestedSwitch++; }
                                else if (ln.Equals("endcase", StringComparison.OrdinalIgnoreCase)) { if (nestedSwitch > 0) nestedSwitch--; else break; }
                                else if (ln.Equals("endif", StringComparison.OrdinalIgnoreCase)) { if (nestedSwitch > 0) nestedSwitch--; else break; }

                                if (nestedSwitch == 0 && !ln.StartsWith("Case ", StringComparison.OrdinalIgnoreCase)) lastSignificant = k;
                                k++;
                            }

                            if (lastSignificant == -1)
                            {
                                errors.Add(new SNILValidationError
                                {
                                    LineNumber = caseHeaderLine + 1,
                                    LineContent = lines[caseHeaderLine],
                                    ErrorType = SNILValidationErrorType.SwitchEmptyBranchBody,
                                    Message = "Case body is empty; each case must contain at least one instruction."
                                });
                            }

                            scan = k;
                            continue;
                        }

                        // If we encounter any other line before cases, skip it
                        scan++;
                    }

                    if (!foundCase)
                    {
                        errors.Add(new SNILValidationError
                        {
                            LineNumber = j + 1,
                            LineContent = lines[j < lines.Length ? j : lines.Length - 1],
                            ErrorType = SNILValidationErrorType.SwitchMissingBranches,
                            Message = "'Switch Show Variant' block must contain at least one 'Case' section."
                        });
                    }

                    // If we reached end of file without finding matching endcase
                    if (scan >= lines.Length || !lines[scan].Trim().Equals("endcase", StringComparison.OrdinalIgnoreCase))
                    {
                        errors.Add(new SNILValidationError
                        {
                            LineNumber = start + 1,
                            LineContent = lines[start],
                            ErrorType = SNILValidationErrorType.SwitchMissingEnd,
                            Message = "'Switch Show Variant' block is not closed with 'endcase'."
                        });
                        i = scan;
                        continue;
                    }

                    // advance i past endcase
                    i = scan;
                }
            }

            return errors;
        }
    }
}