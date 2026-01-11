using System;
using System.Text.RegularExpressions;

namespace SNEngine.Editor.SNILSystem.Validators
{
    public class TemplateBasedInstructionValidator : BaseInstructionValidator
    {
        public override bool CanValidate(string instruction)
        {
            // Проверяем, может ли инструкция соответствовать какому-либо шаблону
            // Исключаем служебные инструкции, такие как name:, function:, call:, end
            string trimmed = instruction.Trim();
            
            // Не обрабатываем специальные инструкции, они проверяются отдельно
            if (Regex.IsMatch(trimmed, @"^name:\s*.+", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(trimmed, @"^function\s+.+", RegexOptions.IgnoreCase) ||
                Regex.IsMatch(trimmed, @"^call\s+.+", RegexOptions.IgnoreCase) ||
                string.Equals(trimmed, "end", StringComparison.Ordinal) ||
                string.Equals(trimmed, "End", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("Jump To ", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmed, "Start", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            
            // Для всех остальных инструкций проверяем, есть ли соответствующий шаблон
            var templates = SNILTemplateManager.GetNodeTemplates();
            foreach (var template in templates)
            {
                var parameters = SNILTemplateMatcher.MatchLineWithTemplate(instruction, template.Value.Template);
                if (parameters != null)
                {
                    return true;
                }
            }
            
            return false;
        }

        public override ValidationInstructionResult Validate(string instruction)
        {
            // Проверяем, соответствует ли инструкция какому-либо шаблону
            var templates = SNILTemplateManager.GetNodeTemplates();
            foreach (var template in templates)
            {
                var parameters = SNILTemplateMatcher.MatchLineWithTemplate(instruction, template.Value.Template);
                if (parameters != null)
                {
                    return ValidationInstructionResult.Ok();
                }
            }
            
            return ValidationInstructionResult.Error($"Unknown instruction format: '{instruction}'");
        }
    }
}