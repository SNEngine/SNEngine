using System;
using System.Text.RegularExpressions;

namespace SNEngine.Editor.SNILSystem.Validators
{
    public class ArithmeticInstructionValidator : BaseInstructionValidator
    {
        public override bool CanValidate(string instruction)
        {
            // Check for arithmetic operations like:
            // - variable++ (increment)
            // - variable-- (decrement)
            // - variable + value
            // - variable - value
            // - variable * value
            // - variable / value
            // - variable % value
            // - variable += value
            // - variable -= value
            // etc.
            string pattern = @"^\w+\s*(\+\+|--|\+=|-=|\*=|/=|%=|\+|\-|\*|/|%)\s*.*$";
            return Regex.IsMatch(instruction.Trim(), pattern);
        }

        public override ValidationInstructionResult Validate(string instruction)
        {
            // Parse the arithmetic instruction to check its format
            var arithmeticInstruction = ParseArithmeticInstruction(instruction.Trim());
            if (arithmeticInstruction == null)
            {
                return ValidationInstructionResult.Error($"Invalid arithmetic instruction format: {instruction}");
            }

            // Validate the variable name
            if (string.IsNullOrEmpty(arithmeticInstruction.VariableName))
            {
                return ValidationInstructionResult.Error($"Invalid variable name in arithmetic instruction: {instruction}");
            }

            // Validate the operator
            if (string.IsNullOrEmpty(arithmeticInstruction.Operator))
            {
                return ValidationInstructionResult.Error($"Invalid operator in arithmetic instruction: {instruction}");
            }

            // For increment/decrement operations, there's no additional value needed
            if (arithmeticInstruction.Operator != "++" && arithmeticInstruction.Operator != "--")
            {
                // Validate the value for other operations
                if (string.IsNullOrEmpty(arithmeticInstruction.Value))
                {
                    return ValidationInstructionResult.Error($"Missing value in arithmetic instruction: {instruction}");
                }

                // Check if the value is a valid number or variable name
                if (!IsValidValue(arithmeticInstruction.Value))
                {
                    return ValidationInstructionResult.Error($"Invalid value '{arithmeticInstruction.Value}' in arithmetic instruction: {instruction}");
                }

                if (arithmeticInstruction.Operator == "/" || arithmeticInstruction.Operator == "%")
                {
                    if (IsNumericValue(arithmeticInstruction.Value) &&
                        double.TryParse(arithmeticInstruction.Value, out double value) &&
                        Math.Abs(value) < double.Epsilon)
                    {
                        return ValidationInstructionResult.Error($"Division by zero is not allowed in arithmetic instruction: {instruction}");
                    }
                }
            }

            return ValidationInstructionResult.Ok();
        }

        private ArithmeticInstruction ParseArithmeticInstruction(string instruction)
        {
            // Handle increment/decrement (e.g., number++)
            var incrementMatch = Regex.Match(instruction, @"^(\w+)\s*(\+\+)$", RegexOptions.IgnoreCase);
            if (incrementMatch.Success)
            {
                return new ArithmeticInstruction
                {
                    VariableName = incrementMatch.Groups[1].Value,
                    Operator = incrementMatch.Groups[2].Value,
                    Value = "1" // Default increment/decrement value
                };
            }

            // Handle decrement (e.g., number--)
            var decrementMatch = Regex.Match(instruction, @"^(\w+)\s*(--)$", RegexOptions.IgnoreCase);
            if (decrementMatch.Success)
            {
                return new ArithmeticInstruction
                {
                    VariableName = decrementMatch.Groups[1].Value,
                    Operator = decrementMatch.Groups[2].Value,
                    Value = "1" // Default increment/decrement value
                };
            }

            // Handle compound assignment (e.g., number += 5)
            var compoundMatch = Regex.Match(instruction, @"^(\w+)\s*([+\-*/%]=)\s*(.+)$", RegexOptions.IgnoreCase);
            if (compoundMatch.Success)
            {
                string op = compoundMatch.Groups[2].Value.Substring(0, 1); // Extract operator without =
                return new ArithmeticInstruction
                {
                    VariableName = compoundMatch.Groups[1].Value,
                    Operator = op,
                    Value = compoundMatch.Groups[3].Value.Trim()
                };
            }

            // Handle simple arithmetic (e.g., number + 5)
            var simpleMatch = Regex.Match(instruction, @"^(\w+)\s*([+\-*/%])\s*(.+)$", RegexOptions.IgnoreCase);
            if (simpleMatch.Success)
            {
                return new ArithmeticInstruction
                {
                    VariableName = simpleMatch.Groups[1].Value,
                    Operator = simpleMatch.Groups[2].Value,
                    Value = simpleMatch.Groups[3].Value.Trim()
                };
            }

            return null;
        }

        private bool IsValidValue(string value)
        {
            // Check if the value is a valid number (int or float) or a valid variable name
            if (double.TryParse(value, out _))
            {
                return true;
            }

            // Check if it's a valid identifier (variable name)
            return Regex.IsMatch(value, @"^[a-zA-Z_]\w*$");
        }

        private bool IsNumericValue(string value)
        {
            // Check if the value is a valid number (int or float)
            return double.TryParse(value, out _);
        }

        private class ArithmeticInstruction
        {
            public string VariableName { get; set; }
            public string Operator { get; set; }
            public string Value { get; set; }
        }
    }
}