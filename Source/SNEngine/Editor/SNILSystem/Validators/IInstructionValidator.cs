using System.Collections.Generic;

namespace SNEngine.Editor.SNILSystem.Validators
{
    public interface IInstructionValidator
    {
        /// <summary>
        /// Проверяет, может ли валидатор проверить данную инструкцию
        /// </summary>
        /// <param name="instruction">Строка инструкции</param>
        /// <returns>True, если валидатор может проверить инструкцию</returns>
        bool CanValidate(string instruction);

        /// <summary>
        /// Проверяет инструкцию
        /// </summary>
        /// <param name="instruction">Строка инструкции</param>
        /// <returns>Результат валидации</returns>
        ValidationInstructionResult Validate(string instruction);
    }

    public class ValidationInstructionResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public List<SNILValidationError> Errors { get; set; } = new List<SNILValidationError>();

        public static ValidationInstructionResult Ok()
        {
            return new ValidationInstructionResult { IsValid = true };
        }

        public static ValidationInstructionResult Error(string message)
        {
            return new ValidationInstructionResult { IsValid = false, ErrorMessage = message };
        }
    }
}