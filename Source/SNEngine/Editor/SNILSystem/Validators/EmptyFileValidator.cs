using System.Collections.Generic;

namespace SNEngine.Editor.SNILSystem.Validators
{
    public class EmptyFileValidator
    {
        public static List<SNILValidationError> ValidateEmptyFile(string[] lines)
        {
            var errors = new List<SNILValidationError>();

            if (lines == null || lines.Length == 0)
            {
                errors.Add(new SNILValidationError
                {
                    LineNumber = 0,
                    LineContent = "",
                    ErrorType = SNILValidationErrorType.EmptyFile,
                    Message = "SNIL script is empty."
                });
            }

            return errors;
        }
    }
}