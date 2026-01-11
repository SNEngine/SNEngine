using System;
using System.Collections.Generic;
using System.Linq;

namespace SNEngine.Editor.SNILSystem.Validators
{
    public class SNILSyntaxValidator : SNILValidator
    {
        public SNILSyntaxValidator()
        {
        }

        public override bool Validate(string[] lines, out string errorMessage)
        {
            return Validate(lines, out errorMessage, out _);
        }

        public bool Validate(string[] lines, out string errorMessage, out List<SNILValidationError> errors)
        {
            errors = new List<SNILValidationError>();
            errorMessage = "";

            // Проверяем на пустой файл
            var emptyFileErrors = EmptyFileValidator.ValidateEmptyFile(lines);
            if (emptyFileErrors.Count > 0)
            {
                errors.AddRange(emptyFileErrors);
                errorMessage = string.Join("\n", errors.Select(e => e.ToString()));
                return false;
            }

            // Используем специализированные валидаторы
            var nameErrors = NameDirectiveValidator.ValidateNameDirective(lines);
            var functionErrors = FunctionValidator.ValidateFunctions(lines);
            var instructionErrors = InstructionValidator.ValidateInstructions(lines);
            var ifBlockErrors = SNILIfShowVariantValidator.Validate(lines);
            var switchBlockErrors = SNILSwitchShowVariantValidator.Validate(lines);

            // Собираем все ошибки
            errors.AddRange(nameErrors);
            errors.AddRange(functionErrors);
            errors.AddRange(instructionErrors);
            errors.AddRange(ifBlockErrors);
            errors.AddRange(switchBlockErrors);

            if (errors.Count > 0)
            {
                errorMessage = string.Join("\n", errors.Select(e => e.ToString()));
                return false;
            }

            return true;
        }
    }

    public class SNILValidationError
    {
        public int LineNumber { get; set; }
        public string LineContent { get; set; }
        public SNILValidationErrorType ErrorType { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return $"Line {LineNumber}: {ErrorType} - {Message} (Content: '{LineContent}')";
        }
    }

    public enum SNILValidationErrorType
    {
        EmptyFile,
        MissingNameDirective,
        NoContent,
        InvalidStart,
        InvalidEnd,
        UnknownNode,
        InvalidJumpToFormat,
        InvalidFunctionDefinition,
        FunctionNotClosed,

        // If Show Variant block specific errors
        IfMissingVariants,
        IfMissingBranches,
        IfMissingEnd,
        IfEmptyBranchBody,

        // Switch Show Variant block specific errors
        SwitchMissingCases,
        SwitchMissingBranches,
        SwitchMissingEnd,
        SwitchEmptyBranchBody,
        SwitchInvalidCase
    }
}