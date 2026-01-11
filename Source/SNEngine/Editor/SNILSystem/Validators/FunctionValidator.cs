using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SNEngine.Editor.SNILSystem.Validators
{
    public class FunctionValidator
    {
        public static List<SNILValidationError> ValidateFunctions(string[] lines)
        {
            var errors = new List<SNILValidationError>();
            int functionDepth = 0;
            int functionStartLine = -1;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (line.StartsWith("function ", StringComparison.OrdinalIgnoreCase))
                {
                    if (functionDepth > 0)
                    {
                        errors.Add(new SNILValidationError
                        {
                            LineNumber = i + 1,
                            LineContent = line,
                            ErrorType = SNILValidationErrorType.InvalidFunctionDefinition,
                            Message = "Nested functions are not allowed."
                        });
                    }
                    else
                    {
                        functionDepth++;
                        functionStartLine = i;

                        // Проверяем, что после "function" идет имя функции
                        string functionName = line.Substring(9).Trim();
                        if (string.IsNullOrEmpty(functionName))
                        {
                            errors.Add(new SNILValidationError
                            {
                                LineNumber = i + 1,
                                LineContent = line,
                                ErrorType = SNILValidationErrorType.InvalidFunctionDefinition,
                                Message = "Function name is required after 'function' keyword."
                            });
                        }
                    }
                }
                // Only consider lowercase "end" as function end, not uppercase "End"
                else if (line.Equals("end", StringComparison.Ordinal))
                {
                    if (functionDepth == 0)
                    {
                        errors.Add(new SNILValidationError
                        {
                            LineNumber = i + 1,
                            LineContent = line,
                            ErrorType = SNILValidationErrorType.InvalidFunctionDefinition,
                            Message = "'end' statement without matching 'function'."
                        });
                    }
                    else
                    {
                        functionDepth--;
                    }
                }
            }

            if (functionDepth > 0)
            {
                errors.Add(new SNILValidationError
                {
                    LineNumber = functionStartLine + 1,
                    LineContent = lines[functionStartLine].Trim(),
                    ErrorType = SNILValidationErrorType.FunctionNotClosed,
                    Message = "Function definition is not closed with 'end' statement."
                });
            }

            return errors;
        }
    }
}