using System.Text.RegularExpressions;

namespace SNEngine.Editor.SNILSystem.Validators
{
    public class SetVariableInstructionValidator : BaseInstructionValidator
    {
        public override bool CanValidate(string instruction)
        {
            // Проверяем, соответствует ли инструкция формату "set [имя] = [значение]"
            string pattern = @"^set\s+\w+\s*=.*$";
            return Regex.IsMatch(instruction.Trim(), pattern, RegexOptions.IgnoreCase);
        }

        public override ValidationInstructionResult Validate(string instruction)
        {
            // Извлекаем имя переменной и значение из инструкции
            var (variableName, valueExpression) = ParseSetInstruction(instruction.Trim());
            if (string.IsNullOrEmpty(variableName))
            {
                return ValidationInstructionResult.Error($"Invalid set instruction format: {instruction}. Expected format: 'set variableName = value'");
            }

            // Проверяем, что имя переменной является допустимым идентификатором
            if (!IsValidIdentifier(variableName))
            {
                return ValidationInstructionResult.Error($"Invalid variable name '{variableName}' in set instruction: {instruction}. Variable names must be valid identifiers.");
            }

            // Проверяем, что после знака равенства есть значение
            if (string.IsNullOrEmpty(valueExpression))
            {
                return ValidationInstructionResult.Error($"Missing value in set instruction: {instruction}. Expected format: 'set variableName = value'");
            }

            // Проверяем, что значение не содержит недопустимые символы (в простом случае)
            // В реальной реализации может потребоваться более сложная проверка
            
            return ValidationInstructionResult.Ok();
        }

        private (string variableName, string valueExpression) ParseSetInstruction(string instruction)
        {
            // Ищем формат "set variableName = value"
            var match = Regex.Match(instruction, @"^set\s+(\w+)\s*=\s*(.*)$", RegexOptions.IgnoreCase);
            if (match.Success && match.Groups.Count > 2)
            {
                return (match.Groups[1].Value.Trim(), match.Groups[2].Value.Trim());
            }
            
            return (null, null);
        }

        private bool IsValidIdentifier(string identifier)
        {
            // Проверяем, что идентификатор соответствует требованиям (буквы, цифры, подчеркивания, начинается с буквы)
            return Regex.IsMatch(identifier, @"^[a-zA-Z_]\w*$");
        }
    }
}