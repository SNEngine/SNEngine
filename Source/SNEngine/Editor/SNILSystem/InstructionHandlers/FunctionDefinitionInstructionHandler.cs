using System.Text.RegularExpressions;

namespace SNEngine.Editor.SNILSystem.InstructionHandlers
{
    public class FunctionDefinitionInstructionHandler : BaseInstructionHandler
    {
        public override bool CanHandle(string instruction)
        {
            // Проверяем, является ли инструкция определением функции
            return Regex.IsMatch(instruction.Trim(), @"^function\s+.+", RegexOptions.IgnoreCase);
        }

        public override InstructionResult Handle(string instruction, InstructionContext context)
        {
            // В новой системе определения функций уже обрабатываются заранее
            // Этот обработчик просто пропускает инструкции определения функций
            return InstructionResult.Ok(new { Type = "FunctionDefinitionSkipped" });
        }
    }
}