using System;
using System.Collections.Generic;
using System.Linq;

namespace SNEngine.Editor.SNILSystem.Validators
{
    public static class SNILIfShowVariantValidator
    {
        public static List<SNILValidationError> Validate(string[] lines)
        {
            var errors = new List<SNILValidationError>();

            for (int i = 0; i < lines.Length; i++)
            {
                var t = lines[i].Trim();
                if (string.IsNullOrEmpty(t) || t.StartsWith("//") || t.StartsWith("#")) continue;

                if (t.Equals("If Show Variant", StringComparison.OrdinalIgnoreCase))
                {
                    int start = i;
                    int j = i + 1;

                    // Find Variants: header
                    while (j < lines.Length && (string.IsNullOrWhiteSpace(lines[j]) || lines[j].TrimStart().StartsWith("//") || lines[j].TrimStart().StartsWith("#"))) j++;
                    if (j >= lines.Length || !lines[j].Trim().StartsWith("Variants", StringComparison.OrdinalIgnoreCase))
                    {
                        errors.Add(new SNILValidationError
                        {
                            LineNumber = start + 1,
                            LineContent = lines[start],
                            ErrorType = SNILValidationErrorType.IfMissingVariants,
                            Message = "'If Show Variant' block must contain a 'Variants:' section."
                        });

                        // Try to continue searching (skip ahead)
                        i = j;
                        continue;
                    }

                    // Collect variants lines
                    j++;
                    var variants = new List<string>();
                    while (j < lines.Length)
                    {
                        var cur = lines[j].Trim();
                        if (string.IsNullOrEmpty(cur) || cur.StartsWith("//") || cur.StartsWith("#")) { j++; continue; }
                        if (cur.EndsWith(":" ) || cur.Equals("endif", StringComparison.OrdinalIgnoreCase)) break;
                        variants.Add(cur);
                        j++;
                    }

                    if (variants.Count == 0)
                    {
                        errors.Add(new SNILValidationError
                        {
                            LineNumber = start + 1,
                            LineContent = lines[start],
                            ErrorType = SNILValidationErrorType.IfMissingVariants,
                            Message = "'Variants:' section must contain at least one variant."
                        });
                    }

                    // Now parse sections until matching endif (support nesting)
                    int nest = 0;
                    bool foundSection = false;
                    int scan = j;
                    while (scan < lines.Length)
                    {
                        var line = lines[scan].Trim();
                        if (string.IsNullOrEmpty(line) || line.StartsWith("//") || line.StartsWith("#")) { scan++; continue; }
                        if (line.Equals("If Show Variant", StringComparison.OrdinalIgnoreCase)) { nest++; scan++; continue; }
                        if (line.Equals("Switch Show Variant", StringComparison.OrdinalIgnoreCase)) { nest++; scan++; continue; }
                        if (line.Equals("endif", StringComparison.OrdinalIgnoreCase))
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

                        if (line.EndsWith(":"))
                        {
                            foundSection = true;
                            // section header
                            int sectionHeaderLine = scan;
                            // find last significant line inside section
                            int k = scan + 1;
                            int nestedIf = 0;
                            int lastSignificant = -1;
                            while (k < lines.Length)
                            {
                                var ln = lines[k].Trim();
                                if (string.IsNullOrEmpty(ln) || ln.StartsWith("//") || ln.StartsWith("#")) { k++; continue; }
                                if (ln.EndsWith(":" ) && nestedIf == 0) break; // next section
                                if (ln.Equals("If Show Variant", StringComparison.OrdinalIgnoreCase)) { nestedIf++; }
                                else if (ln.Equals("Switch Show Variant", StringComparison.OrdinalIgnoreCase)) { nestedIf++; }
                                else if (ln.Equals("endif", StringComparison.OrdinalIgnoreCase)) { if (nestedIf > 0) nestedIf--; else break; }
                                else if (ln.Equals("endcase", StringComparison.OrdinalIgnoreCase)) { if (nestedIf > 0) nestedIf--; else break; }

                                if (nestedIf == 0 && !ln.EndsWith(":")) lastSignificant = k;
                                k++;
                            }

                            if (lastSignificant == -1)
                            {
                                errors.Add(new SNILValidationError
                                {
                                    LineNumber = sectionHeaderLine + 1,
                                    LineContent = lines[sectionHeaderLine],
                                    ErrorType = SNILValidationErrorType.IfEmptyBranchBody,
                                    Message = "Branch body is empty; each branch must contain at least one instruction."
                                });
                            }

                            scan = k;
                            continue;
                        }

                        // If we encounter any other line before sections, skip it
                        scan++;
                    }

                    if (!foundSection)
                    {
                        errors.Add(new SNILValidationError
                        {
                            LineNumber = j + 1,
                            LineContent = lines[j < lines.Length ? j : lines.Length - 1],
                            ErrorType = SNILValidationErrorType.IfMissingBranches,
                            Message = "'If Show Variant' block must contain at least one labeled section (True:/False: or variant-name:)."
                        });
                    }

                    // If we reached end of file without finding matching endif
                    if (scan >= lines.Length || !lines[scan].Trim().Equals("endif", StringComparison.OrdinalIgnoreCase))
                    {
                        errors.Add(new SNILValidationError
                        {
                            LineNumber = start + 1,
                            LineContent = lines[start],
                            ErrorType = SNILValidationErrorType.IfMissingEnd,
                            Message = "'If Show Variant' block is not closed with 'endif'."
                        });
                        i = scan;
                        continue;
                    }

                    // advance i past endif
                    i = scan;
                }
            }

            return errors;
        }
    }
}